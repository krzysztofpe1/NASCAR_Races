using NASCAR_Races_Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Nascar_Races_Client
{
    internal class ClientTCPHandler
    {
        private const int _dataPort = 2000;
        private const int _commPort = 2001;
        private const string _serverIP = "127.0.0.1";
        private TcpClient _dataClient;
        private TcpClient _commClient;
        private NetworkStream _dataStream;
        private NetworkStream _commStream;
        private Thread _dataThread;
        private Thread _commThread;
        public bool IsDisposable { get; private set; } = false;
        private BinaryFormatter _binaryFormatter;
        public Car MyCar { get; private set; }
        private List<CarMapper> _opponents { get; set; }
        public Thread CarThread { get; private set; }

        private Point _startingPos = new Point();
        private Point _pitPos = new Point();
        private WorldInformation _worldInformation;
        private int _carNumber;
        public ClientTCPHandler(WorldInformation worldInf)
        {
            _binaryFormatter = new BinaryFormatter();

            _opponents = new();
            if (Connect())
            {
                byte[] comm = new byte[54];
                _commStream.Read(comm);
                using (var ms = new MemoryStream(comm))
                {
                    var formatter = new BinaryFormatter();
                    int deserialized = (int)formatter.Deserialize(ms);
                    _worldInformation = worldInf;
                    _carNumber = deserialized;
                    int temp = 0;
                    while (temp++ < deserialized)
                    {
                        _startingPos = RaceManager.NextStartingPoint();
                        _pitPos = RaceManager.NextPitPoint();
                    }
                    MyCar = new(_startingPos, _pitPos, 1000, deserialized.ToString(), 30000, worldInf);
                    CarThread = new(MyCar.Move);
                }
                Debug.Write(" TUTAJ WESZŁO");
                CarThread = new(MyCar.Move);
                CarThread.Start();
                _dataThread = new(ExchangeData);
                _dataThread.Start();
                _commThread = new(ExchangeComm);
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
                MyCar.CreateMap()
            };
            cars.AddRange(_opponents);

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
        private void ExchangeData()
        {
            while (!IsDisposable)
            {
                //sending my object to server every iteration
                var myObjectSerialized = SerializeCar();
                _dataStream.Write(myObjectSerialized, 0, myObjectSerialized.Length);
                Debug.WriteLine(myObjectSerialized.Length);

            }
        }
        private void ExchangeComm()
        {
            while (!IsDisposable)
            {
                byte[] comm = new byte[54];
                _commStream.Read(comm);
                using (var ms = new MemoryStream(comm))
                {
                    var formatter = new BinaryFormatter();
                    int deserialized = (int)formatter.Deserialize(ms);
                    switch(deserialized)
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
                        default: break;
                    }
                }
            }
        }
        private void RestartCar()
        {
            MyCar.IsDisposable = true;
            MyCar = new(_startingPos, _pitPos, 1000, _carNumber.ToString(), 30000, _worldInformation);
            CarThread = new(MyCar.Move);
        }
        private byte[] SerializeCar()
        {
            using (MemoryStream stream = new())
            {
                var formatter = new BinaryFormatter();
                CarMapper map = MyCar.CreateMap();
                _binaryFormatter.Serialize(stream, map);
                return stream.ToArray();
            }
        }

        public void Dispose()
        {
            _dataClient.Dispose();
            _commClient.Dispose();
            IsDisposable = true;
            MyCar.IsDisposable = true;
        }
    }
}
