using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;

namespace NASCAR_Races_Server
{
    internal class RaceManager
    {
        public List<ServerTCPHandler> ListOfCarHandlers { get; private set; }

        private int _canvasWidth;
        private int _canvasHeight;
        //private int _penCircuitSize;

        private int _straightLength;
        private int _turnRadius;
        private int _pitPosY;
        private int _turnCurveRadius;

        private int _nextCarNumber=1;

        private int _firstRow;
        private int _secondRow;

        private Thread _collisionCheckerThread;
        private Thread _tcpListenerThread;
        private bool _killCollisionChecker = false;
        private static string _serverIP { get; } = "127.0.0.1";
        private static int _dataPort { get; } = 2000;
        private static int _commPort { get; } = 2001;

        private static Point _nextStartingPos;
        private static Point _nextPitPos;
        public bool IsRaceStarted = false;

        public WorldInformation Worldinformation { get; }

        public RaceManager(int straightLength, int turnRadius, int pitPosY, int turnCurveRadius, int penCircuitSize, int penCarSize, PictureBox mainPictureBox)
        {
            ListOfCarHandlers = new List<ServerTCPHandler>();
            _canvasWidth = mainPictureBox.Width;
            _canvasHeight = mainPictureBox.Height;
            //_penCircuitSize = penCircuitSize;

            _straightLength = straightLength;
            _turnRadius = turnRadius;
            _pitPosY = pitPosY;
            _turnCurveRadius = turnCurveRadius;

            _collisionCheckerThread = new(CheckCollisions);
            _tcpListenerThread = new(AwaitForCars);
            _tcpListenerThread.Start();

            Worldinformation = new WorldInformation(straightLength, turnRadius, pitPosY, turnCurveRadius, penCircuitSize, penCarSize, 100, mainPictureBox);
        }
        public List<CarMapper> getCars()
        {
            var temp = new List<CarMapper>();
            ListOfCarHandlers.ForEach(handler => { temp.Add(handler.GetCar()); });
            return temp;
        }

        private void CheckCollisions()
        {
            //creating list of cars
            List<CarMapper>listOfCars = new List<CarMapper>();
            foreach(var handler in ListOfCarHandlers)
            {
                listOfCars.Add(handler.GetCar());
            }
            while (!_killCollisionChecker)
            {
                foreach (CarMapper car1 in listOfCars)
                {
                    if (car1.State != CarMapper.STATE.ON_CIRCUIT && car1.State != CarMapper.STATE.ON_WAY_TO_PIT_STOP)
                        continue;

                    foreach (CarMapper car2 in listOfCars)
                    {
                        if (car1 == car2 || car1.IsDisposable || car2.IsDisposable)
                            continue;

                        if (AreRectanglesColliding(car1, car2))
                        {
                            car1.IsDisposable = true;
                            car2.IsDisposable = true;
                            Debug.WriteLine("Zabito: " + car1.CarName + ", " + car2.CarName);
                        }
                    }
                }
            }
        }

        private void AwaitForCars()
        {
            TcpListener dataServer = new(IPAddress.Parse(_serverIP), _dataPort);
            TcpListener commServer = new(IPAddress.Parse(_serverIP), _commPort);
            dataServer.Start();
            commServer.Start();

            while (true)
            {
                TcpClient dataClient = dataServer.AcceptTcpClient();
                TcpClient commClient = commServer.AcceptTcpClient();
                Debug.WriteLine("Connected");
                var temp = new ServerTCPHandler(dataClient, commClient, _nextCarNumber++);
                ListOfCarHandlers.Add(temp);
                Thread.Sleep(1000);
                if(IsRaceStarted)
                {
                    temp.Start();
                }
            }
        }

        private bool AreRectanglesColliding(CarMapper car1, CarMapper car2)
        {
            double car1MinX = car1.X - car1.Length / 2;
            double car1MaxX = car1.X + car1.Length / 2;
            double car1MinY = car1.Y - car1.Width / 2;
            double car1MaxY = car1.Y + car1.Width / 2;

            double car2MinX = car2.X - car2.Length / 2;
            double car2MaxX = car2.X + car2.Length / 2;
            double car2MinY = car2.Y - car2.Width / 2;
            double car2MaxY = car2.Y + car2.Width / 2;

            // Rotate the coordinates of the second car by the angle of the first car
            double rotationAngle = car1.HeadingAngle;
            double cosAngle = Math.Cos(rotationAngle);
            double sinAngle = Math.Sin(rotationAngle);

            double car2RotatedMinX = (car2MinX - car1.X) * cosAngle - (car2MinY - car1.Y) * sinAngle + car1.X;
            double car2RotatedMaxX = (car2MaxX - car1.X) * cosAngle - (car2MaxY - car1.Y) * sinAngle + car1.X;
            double car2RotatedMinY = (car2MinX - car1.X) * sinAngle + (car2MinY - car1.Y) * cosAngle + car1.Y;
            double car2RotatedMaxY = (car2MaxX - car1.X) * sinAngle + (car2MaxY - car1.Y) * cosAngle + car1.Y;

            if (car1MaxX < car2RotatedMinX || car1MinX > car2RotatedMaxX)
                return false;

            if (car1MaxY < car2RotatedMinY || car1MinY > car2RotatedMaxY)
                return false;

            return true;
        }
        private Point NextStartingPoint()
        {
            Point tempPoint = new(_nextStartingPos.X, _nextStartingPos.Y);
            _nextStartingPos.X -= _straightLength / 30;
            _nextStartingPos.Y = (_nextStartingPos.Y == _firstRow) ? _secondRow : _firstRow;
            return tempPoint;
        }
        private Point NextPitPoint()
        {
            Point temp = new(_nextPitPos.X, _nextPitPos.Y);
            _nextPitPos.X -= Worldinformation.CarLength * 4;
            return temp;
        }

        public void StartRace()
        {
            IsRaceStarted = true;
            ListOfCarHandlers.ForEach(handler => { handler.Start(); });
            //_collisionCheckerThread.Start();
        }
        public void KillThreads()
        {
            _killCollisionChecker = true;
        }
    }
}
