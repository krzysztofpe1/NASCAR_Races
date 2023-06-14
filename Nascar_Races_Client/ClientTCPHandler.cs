using NASCAR_Races_Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Nascar_Races_Client
{
    internal class ClientTCPHandler
    {
        private const int _dataPort = 2000;
        private const int _commPort = 2001;
        private const string _serverIP = "192.168.0.100";
        private TcpClient _dataClient;
        private TcpClient _commClient;
        private NetworkStream _dataStream;
        private NetworkStream _commStream;
        private Thread _sendingDataThread;
        private Thread _receivingDataThread;
        private Thread _commThread;
        public bool IsDisposable { get; private set; } = false;
        private BinaryFormatter _binaryFormatter;
        private DataContractSerializer _serializer;
        public Car MyCar { get; private set; }
        public List<CarMapper> Opponents { get; private set; }
        public Thread CarThread { get; private set; }

        private Point _startingPos = new Point();
        private Point _pitPos = new Point();
        private WorldInformation _worldInformation;
        private int _carNumber;

        private bool _serverReadyForNextData = true;
        public ClientTCPHandler(WorldInformation worldInf)
        {
            _binaryFormatter = new BinaryFormatter();
            _serializer = new(typeof(CarMapper));

            Opponents = new();
            if (Connect())
            {
                int response = _commStream.ReadByte();
                if (response < 0) { }
                _worldInformation = worldInf;
                _carNumber = response;
                int temp = 0;
                while (temp++ < response)
                {
                    _startingPos = RaceManager.NextStartingPoint();
                    _pitPos = RaceManager.NextPitPoint();
                }
                MyCar = new(_startingPos, _pitPos, 1000, response.ToString(), 30000, worldInf);
                CarThread = new(MyCar.Move);
                CarThread.Start();

                _sendingDataThread = new(SendingData);
                _sendingDataThread.Start();

                _receivingDataThread = new(ReceivingData);
                _receivingDataThread.Start();

                _commThread = new(ReceivingComm);
                _commThread.Start();
            }
            else
            {

            }
        }
        public List<DrawableCar> GetCars()
        {
            List<DrawableCar> cars = new List<DrawableCar>
            {
                (DrawableCar)MyCar
            };
            cars.AddRange(Opponents);

            return cars;
        }

        private bool Connect()
        {
            try
            {
                _dataClient = new TcpClient(_serverIP, _dataPort);
                _dataStream = _dataClient.GetStream();

                _commClient = new TcpClient(_serverIP, _commPort);
                _commStream = _commClient.GetStream();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private void SendingData()
        {
            while (!IsDisposable)
            {
                //sending my object to server every iteration
                Thread.Sleep(10);
                if (!_serverReadyForNextData) continue;
                _serverReadyForNextData = false;
                var myObjectSerialized = SerializeCar();
                _dataStream.Write(myObjectSerialized, 0, myObjectSerialized.Length);
            }
        }
        private async void ReceivingData()
        {
            {
                byte[] buffer = new byte[256];
                List<byte> data = new List<byte>();
                int lastBytesRead = 0;
                while (!IsDisposable)
                {
                    try
                    {
                        int bytesRead = _dataStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead < lastBytesRead)
                        {
                            if (bytesRead != 0) data.AddRange(buffer.Take(bytesRead));
                            break;
                        }
                        lastBytesRead = bytesRead;
                        data.AddRange(buffer.Take(bytesRead));
                    }
                    catch (Exception ex) { IsDisposable = true; break; }
                }
                Opponents = Deserialize<List<CarMapper>>(data.ToArray());
                MyCar.NeighbouringCars = Opponents;
                SendComm(TCPSignals.clientReadyForData);
            }
        }
        private void ReceivingComm()
        {
            while (!IsDisposable)
            {
                int response = _commStream.ReadByte();
                if (response < 0) continue;
                switch (response)
                {
                    case TCPSignals.startRaceSignal:
                        MyCar.Started = true;
                        break;
                    case TCPSignals.endRaceSignal:
                        RestartCar();
                        break;
                    case TCPSignals.killCarSignal:
                        MyCar.IsDisposable = true;
                        break;
                    case TCPSignals.serverReadyForData:
                        _serverReadyForNextData = true;
                        break;
                    default: break;
                }
            }
        }
        private void SendComm(int signal)
        {
            _commStream.WriteByte((byte)signal);
        }
        private void RestartCar()
        {
            MyCar.IsDisposable = true;
            MyCar = new(_startingPos, _pitPos, 1000, _carNumber.ToString(), 30000, _worldInformation);
            CarThread = new(MyCar.Move);
        }
        private byte[] SerializeCar()
        {
            MemoryStream ms = new();
            _binaryFormatter.Serialize(ms, MyCar.CreateMap());
            return ms.ToArray();
        }
        private T Deserialize<T>(byte[] data)
        {
            if (data == null) return default(T);
            using (MemoryStream ms = new(data))
            {
                return (T)_binaryFormatter.Deserialize(ms);
            }
        }

        public void Dispose()
        {
            MyCar.IsDisposable = true;
            SendComm(TCPSignals.killCarSignal);
            Thread.Sleep(500);
            IsDisposable = true;
            _dataClient.Dispose();
            _commClient.Dispose();
        }
    }
}
