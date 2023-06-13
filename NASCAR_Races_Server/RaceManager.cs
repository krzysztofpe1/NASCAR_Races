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
using System.Reflection.Metadata;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

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
            while (!_killCollisionChecker)
            {
                Dictionary<CarMapper, ServerTCPHandler> handlerMap = new();
                ListOfCarHandlers.ForEach(handler =>
                {
                    handlerMap[handler.GetCar()] = handler;
                });
                foreach (KeyValuePair<CarMapper, ServerTCPHandler> pair1 in handlerMap)
                {
                    var car1 = pair1.Key;
                    if (car1.State != CarMapper.STATE.ON_CIRCUIT && car1.State != CarMapper.STATE.ON_WAY_TO_PIT_STOP)
                        continue;
                    foreach(KeyValuePair<CarMapper, ServerTCPHandler> pair2 in handlerMap)
                    {
                        var car2 = pair2.Key;
                        if (car1 == car2 || car1.IsDisposable || car2.IsDisposable)
                            continue;

                        if (AreCarsColliding(car1, car2))
                        {
                            handlerMap[car1].Kill();
                            handlerMap[car2].Kill();
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
                temp.AllCarHandlers = ListOfCarHandlers;
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
        private (double, double) rotateCar(double X, double Y, double Angle, double centerX, double centerY)
        {
            Angle = Angle * Math.PI / 180;
            double newX = centerX + (X - centerX ) * Math.Cos(Angle) - (Y - centerY) * Math.Sin(Angle);
            double newY = centerY + (X - centerX ) * Math.Sin(Angle) + (Y - centerY) * Math.Cos(Angle);
            return (newX, newY);
        }

        private bool IsSeparatingAxis(List<(double, double)> car1Points, List<(double, double)> car2Points, (double, double) axis)
        {
            // Project points onto axis
            List<double> car1Projections = car1Points.Select(point => point.Item1 * axis.Item1 + point.Item2 * axis.Item2).ToList();
            List<double> car2Projections = car2Points.Select(point => point.Item1 * axis.Item1 + point.Item2 * axis.Item2).ToList();

            // Check for overlap
            if (car1Projections.Max() < car2Projections.Min() || car2Projections.Max() < car1Projections.Min())
                return true; // There is no overlap along this axis, it's a separating axis

            return false;
        }
        // 
        private bool AreCarsColliding(CarMapper car1, CarMapper car2)
        {
            // A---------B
            // |         |--->
            // |         |--->
            // C---------D
            double rotationAngleCar1 = Math.Abs(car1.HeadingAngle);

            //Console.WriteLine(rotationAngleCar1);
            //Debug.WriteLine(rotationAngleCar1);

            (double, double) car1A = (car1.X - car1.Length / 2, car1.Y - car1.Width / 2);
            (double, double) car1B = (car1.X + car1.Length / 2, car1.Y - car1.Width / 2);
            (double, double) car1C = (car1.X - car1.Length / 2, car1.Y + car1.Width / 2);
            (double, double) car1D = (car1.X + car1.Length / 2, car1.Y + car1.Width / 2);

            car1A = rotateCar(car1A.Item1, car1A.Item2, rotationAngleCar1, car1.X, car1.Y);
            car1B = rotateCar(car1B.Item1, car1B.Item2, rotationAngleCar1, car1.X, car1.Y);
            car1C = rotateCar(car1C.Item1, car1C.Item2, rotationAngleCar1, car1.X, car1.Y);
            car1D = rotateCar(car1D.Item1, car1D.Item2, rotationAngleCar1, car1.X, car1.Y);

            double rotationAngleCar2 = car1.HeadingAngle;

            (double, double) car2A = (car2.X - car2.Length / 2, car2.Y - car2.Width / 2);
            (double, double) car2B = (car2.X + car2.Length / 2, car2.Y - car2.Width / 2);
            (double, double) car2C = (car2.X - car2.Length / 2, car2.Y + car2.Width / 2);
            (double, double) car2D = (car2.X + car2.Length / 2, car2.Y + car2.Width / 2);

            car2A = rotateCar(car2A.Item1, car2A.Item2, rotationAngleCar2, car2.X, car2.Y);
            car2B = rotateCar(car2B.Item1, car2B.Item2, rotationAngleCar2, car2.X, car2.Y);
            car2C = rotateCar(car2C.Item1, car2C.Item2, rotationAngleCar2, car2.X, car2.Y);
            car2D = rotateCar(car2D.Item1, car2D.Item2, rotationAngleCar2, car2.X, car2.Y);

            // Corners of both cars
            List<(double, double)> car1Points = new List<(double, double)> { car1A, car1B, car1C, car1D };
            List<(double, double)> car2Points = new List<(double, double)> { car2A, car2B, car2C, car2D };

            double a1 = 0;
            double b1 = 0;
            double a2 = 0;
            double b2 = 0;
            if (car1A.Item1 != car1B.Item1)
            {
                a1 = (car1A.Item2 - car1B.Item2) / (car1A.Item1 - car1B.Item1);
                b1 = (car1A.Item2 - a1 * car1A.Item1);
                if (rotationAngleCar1 < 90 || rotationAngleCar1 > 270)
                    b1 += car1.Width / 2;
                else
                    b1 -= car1.Width / 2;

                a2 = -1 / a1;
                b2 = car1.Y - a1 * car1.X;

                if (a1 == 0)
                {
                    foreach ((double, double) point in car2Points)
                    {
                        double distance1 = Math.Abs(point.Item1 - car1.X);
                        double distance2 = Math.Abs(point.Item2 - car1.Y);
                        if (distance2 <= car1.Width / 2 && distance1 <= car1.Length / 2)
                            return true;
                    }
                }
                else
                {
                    foreach ((double, double) point in car2Points)
                    {
                        // obliczy odległość od prostej
                        double distance1 = Math.Abs(point.Item1 * a1 + point.Item2 * (-1) + b1) / Math.Sqrt(a1 * a1 + 1);
                        double distance2 = Math.Abs(point.Item1 * a2 + point.Item2 * (-1) + b2) / Math.Sqrt(a2 * a2 + 1);
                        if (distance2 <= car1.Width / 2 && distance1 <= car1.Length / 2)
                            return true;
                    }
                }

            }


            return false;
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
            _collisionCheckerThread.Start();
        }
        public void KillThreads()
        {
            _killCollisionChecker = true;
        }
    }
}
