using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races_Server
{
    internal class ServerTCPHandler
    {
        private static int startRaceSignal { get; } = 420;
        private TcpClient _dataClient;
        private TcpClient _commClient;
        private NetworkStream _dataStream;
        private NetworkStream _commStream;

        private CarMapper _myCar;
        private Thread _ReceivingDataThread;
        private Thread _SendingDataThread;
        private Thread _commThread;
        private BinaryFormatter _formatter;
        private DataContractSerializer _serializer;
        private int _dataLength = 1024;

        public bool IsDisposable = false;

        private bool _clientReadyForNextData=true;

        public List<ServerTCPHandler> AllCarHandlers { get; set; }
        public ServerTCPHandler(TcpClient dataClient, TcpClient commClient, int myCarNumber)
        {
            _dataClient = dataClient;
            _dataStream = _dataClient.GetStream();

            _commClient = commClient;
            _commStream = _commClient.GetStream();

            _myCar = new();

            _formatter = new();
            _serializer = new(typeof(CarMapper));
            _dataLength = DefineBufferSizeForCarMapperRawData();

            _ReceivingDataThread = new(ReceivingData);
            _ReceivingDataThread.Start();

            _SendingDataThread = new(SendingData);
            _SendingDataThread.Start();

            _commThread = new(ReceivingComm);
            _commThread.Start();
            SendingComm(myCarNumber);
        }
        private void ReceivingData()
        {
            while (!IsDisposable)
            {
                byte[] response = new byte[_dataLength];
                int bytesRead = _dataStream.Read(response, 0, _dataLength);
                var temp = DeserializeCar(response);
                _myCar = temp;
                SendingComm(TCPSignals.serverReadyForData);
            }
        }
        private void SendingData()
        {
            while (!IsDisposable)
            {
                if (!_clientReadyForNextData) continue;
                Thread.Sleep(50);
                List<CarMapper> cars = new List<CarMapper>();
                AllCarHandlers.ForEach(carHandler =>
                { 
                    if(carHandler != this)
                        cars.Add(carHandler.GetCar()); 
                });
                byte[] dataToSend = Serialize(cars);
                _dataStream.Write(dataToSend, 0, dataToSend.Length);
            }
        }
        private void ReceivingComm()
        {
            while (!IsDisposable)
            {
                byte[] comm = new byte[54];
                _commStream.Read(comm);
                using (var ms = new MemoryStream(comm))
                {
                    var formatter = new BinaryFormatter();
                    int deserialized = (int)formatter.Deserialize(ms);
                    switch (deserialized)
                    {
                        case TCPSignals.clientReadyForData:
                            _clientReadyForNextData = true;
                            break;
                        default: break;
                    }
                }
            }
        }
        private void SendingComm(int signal)
        {
            using (var ms = new MemoryStream())
            {
                _formatter.Serialize(ms, signal);
                _commStream.Write(ms.ToArray());
            }
        }
        private byte[] Serialize(object obj)
        {
            if (obj == null) return null;
            using (MemoryStream stream = new())
            {
                _formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }
        private int DefineBufferSizeForCarMapperRawData()
        {
            using (MemoryStream stream = new())
            {
                var formatter = new BinaryFormatter();
                CarMapper temp = new()
                {
                    IsDisposable = true,
                    CarName = "1",
                    MaxHorsePower = 1,
                    X = 0,
                    Y = 0,
                    Length = 0,
                    Width = 0,
                    Speed = 0,
                    HeadingAngle = 0,
                    FuelMass = 0,
                    FuelBurningRatio = 0,
                    CurrentHorsePower = 0,
                    State = CarMapper.STATE.ON_WAY_TO_PIT_STOP
                };
                _formatter.Serialize(stream, temp);
                return stream.ToArray().Length;
            }
        }
        private CarMapper DeserializeCar(byte[] response)
        {
            using (MemoryStream ms = new(response))
            {
                return (CarMapper)_formatter.Deserialize(ms);
            }
        }
        public CarMapper GetCar()
        {
            return _myCar;
        }
        public void Start()
        {
            SendingComm(TCPSignals.startRaceSignal);
        }
        public void Kill()
        {
            SendingComm(TCPSignals.killCarSignal);
        }
    }
}
