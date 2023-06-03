using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace NASCAR_Races
{
    public class RaceManager
    {
        public List<CarThread> ListOfCarThreads;
        public List<Car> ListOfCars;

        private int _canvasWidth;
        private int _canvasHeight;
        //private int _penCircuitSize;

        private int _straightLength;
        private int _turnRadius;
        private int _pitPosY;
        private int _turnCurveRadius;

        private Point _nextStartingPos;
        private Point _nextPitPos;
        private int _firstRow;
        private int _secondRow;

        private Thread _thread;
        private bool _killCollisionChecker = false;

        public Worldinformation Worldinformation { get; }

        public RaceManager(int straightLength, int turnRadius, int pitPosY, int turnCurveRadius, int penCircuitSize, int penCarSize, PictureBox mainPictureBox)
        {
            _canvasWidth = mainPictureBox.Width;
            _canvasHeight = mainPictureBox.Height;
            //_penCircuitSize = penCircuitSize;

            _straightLength = straightLength;
            _turnRadius = turnRadius;
            _pitPosY = pitPosY;
            _turnCurveRadius = turnCurveRadius;

            _nextStartingPos = new Point();
            _firstRow = _canvasHeight / 2 + _turnRadius + penCircuitSize / 2;
            _secondRow = _canvasHeight / 2 + _turnRadius - penCircuitSize / 2;
            _nextStartingPos.X = _canvasWidth / 2 + straightLength / 4;
            _nextStartingPos.Y = _firstRow;


            _thread = new(CheckCollisions);

            Worldinformation = new Worldinformation(straightLength, turnRadius, pitPosY, turnCurveRadius, penCircuitSize, penCarSize, 100, mainPictureBox);

            _nextPitPos = new Point();
            _nextPitPos.Y = pitPosY - penCircuitSize / 4 + Worldinformation.CarWidth / 2;
            Worldinformation.CarWidthOfPittingManouver = pitPosY - _nextPitPos.Y;
            _nextPitPos.X = Worldinformation.x2 - Worldinformation.CarLength;
        }

        public List<Car> CreateListOfCars()
        {
            Random random = new Random();
            ListOfCarThreads = new List<CarThread>();
            ListOfCars = new List<Car>();
            for (int i = 0; i < Worldinformation.NumberOfCars; i++)
            {
                CarThread car = new(NextStartingPoint(), NextPitPoint(), 1000, i.ToString(), (random.NextDouble() <= 0.5) ? 30000 : 15000 + (float)random.NextDouble() * 10000, Worldinformation);
                //CarThread car = new(NextStartingPoint(), NextPitPoint(), 1000, i.ToString(), (random.NextDouble() <= 0.5) ? 30000 : 25000 + (float)random.NextDouble() * 5000, Worldinformation);
                //Debug.WriteLine();
                ListOfCarThreads.Add(car);
                ListOfCars.Add((Car)car);
            }
            Worldinformation.ListOfCars = ListOfCars;
            return ListOfCars;
        }

        private void CheckCollisions()
        {
            while (!_killCollisionChecker)
            {
                foreach (Car car1 in ListOfCars)
                {
                    if (car1.State != Car.STATE.ON_CIRCUIT && car1.State != Car.STATE.ON_WAY_TO_PIT_STOP)
                        continue;

                    foreach (Car car2 in ListOfCars)
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

        private bool AreRectanglesColliding(Car car1, Car car2)
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
            ListOfCarThreads.ForEach(carThread => { carThread.StartCar(); });
            _thread.Start();
        }
        public void KillThreads()
        {
            _killCollisionChecker = true;
            ListOfCarThreads.ForEach(carThread => { carThread.Kill(); });
        }
    }
}
