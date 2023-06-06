using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
        public CarClientHandler(TcpClient dataClient, TcpClient commClient, Point startingPoint, Point pitPoint)
        {
            _dataClient = dataClient;
            _dataStream = _dataClient.GetStream();

            _commClient = commClient;
            _commStream = _commClient.GetStream();

            _startingPoint = startingPoint;
            _pitPoint = pitPoint;
        }
        private void CreateCar(Point point, Point pitPos, float weight, string carName, float maxHorsePower, WorldInformation worldinformation)
        {
            _myCar = new(point.X, point.Y, weight, carName, maxHorsePower, worldinformation);
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
