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
        private Thread _dataThread;
        private Thread _commThread;
        private BinaryFormatter _formatter;
        private int _dataLength;
        public ServerTCPHandler(TcpClient dataClient, TcpClient commClient, int myCarNumber)
        {
            _dataClient = dataClient;
            _dataStream = _dataClient.GetStream();

            _commClient = commClient;
            _commStream = _commClient.GetStream();

            _myCar = new();

            _formatter = new();
            _dataLength = DefineBufferSizeForCarMapperRawData();

            _dataThread = new(ExchangeData);
            _dataThread.Start();

            _commThread = new(ExchangeComm);
            //_commThread.Start();
            SendComm(myCarNumber);
        }
        private void ExchangeData()
        {
            while (true)
            {
                byte[] response = new byte[_dataLength];
                int bytesRead = _dataStream.Read(response, 0, _dataLength);
                var temp = DeserializeCar(response);
                _myCar = temp;
            }
        }
        private void ExchangeComm()
        {
            while (true)
            {

            }
        }
        private void SendComm(int signal)
        {
            using (var ms = new MemoryStream())
            {
                _formatter.Serialize(ms, signal);
                _commStream.Write(ms.ToArray());
                Debug.WriteLine(ms.ToArray().Length);
            }
        }
        private byte[] SerializeCar()
        {
            using (MemoryStream stream = new())
            {
                var formatter = new BinaryFormatter();
                _formatter.Serialize(stream, _myCar);
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
            using(MemoryStream memoryStream = new MemoryStream(response))
            {
                var myobject = (CarMapper)_formatter.Deserialize(memoryStream);
                return myobject;
            }
        }
        public CarMapper GetCar()
        {
            return _myCar;
        }
        public void Start()
        {
            SendComm(startRaceSignal);
        }
    }
}
