using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races_Server
{
    public class CarClientHandler
    {
        private TcpClient _dataClient;
        private TcpClient _commClient;
        private NetworkStream _dataStream;
        private NetworkStream _commStream;
        private Point _startingPoint { get; set; }
        private Point _pitPoint { get; set; }
        private Car _myCar;
        private Thread _thread;
        private BinaryFormatter _binaryFormatter;
        public CarClientHandler(TcpClient dataClient, TcpClient commClient, Point startingPoint, Point pitPoint)
        {
            _dataClient = dataClient;
            _dataStream = _dataClient.GetStream();

            _commClient = commClient;
            _commStream = _commClient.GetStream();

            _startingPoint = startingPoint;
            _pitPoint = pitPoint;
            _thread = new(ExchangeData);
            _binaryFormatter = new BinaryFormatter();
        }
        private void CreateCar(Point point, Point pitPos, float weight, string carName, float maxHorsePower, WorldInformation worldinformation)
        {
            _myCar = new(point.X, point.Y, weight, carName, maxHorsePower, worldinformation);
        }
        private void ExchangeData()
        {
            while (true)
            {
                byte[] response = new byte[1024];
                int bytesRead = _dataStream.Read(response, 0, response.Length);
                var temp = DeserializeData(response);
                _myCar.CopyMapper(temp);
            }
        }
        private CarMapper DeserializeData(byte[] response)
        {
            using(MemoryStream memoryStream = new MemoryStream(response))
            {
                return (CarMapper) _binaryFormatter.Deserialize(memoryStream);
            }
        }
        public Car GetCar()
        {
            return _myCar;
        }
        public void Start()
        {

        }
    }
}
