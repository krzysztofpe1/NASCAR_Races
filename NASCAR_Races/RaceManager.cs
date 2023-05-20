using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _nextPitPos.Y = pitPosY - penCircuitSize / 4;
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
                    if (car1.State != Car.STATE.ON_CIRCUIT && car1.State != Car.STATE.ON_WAY_TO_PIT_STOP) continue;
                    foreach (Car car2 in ListOfCars)
                    {
                        if (car1 == car2) continue;
                        if (car1.IsDisposable || car2.IsDisposable) continue;
                        if (Math.Abs(car1.X - car2.X) < car1.Length / 2 + car2.Length / 2 && Math.Abs(car1.Y - car2.Y) < car1.Width / 2 + car2.Width / 2)
                        {
                            double diagonal = Math.Sqrt(Math.Pow(car1.Length, 2) + Math.Pow(car1.Width, 2));
                            double diagonal2 = Math.Sqrt(Math.Pow(car2.Length, 2) + Math.Pow(car2.Width, 2));
                            double angle1 = (car1.HeadingAngle + 90) % 360;
                            double angle2 = (car2.HeadingAngle + 90) % 360;
                            double diffAngle = Math.Abs(angle1 - angle2);
                            if (diffAngle > 180) diffAngle = 360 - diffAngle;
                            double distance = Math.Sqrt(Math.Pow(car2.X - car1.X, 2) + Math.Pow(car2.Y - car1.Y, 2));
                            double diagonalSum = diagonal + diagonal2;
                            if (distance <= diagonalSum && Math.Abs(diffAngle) < 90)
                            {
                                car1.IsDisposable = true;
                                car2.IsDisposable = true;
                                Debug.WriteLine("Zabito: " + car1.CarName + ", " + car2.CarName);
                            }
                        }
                    }
                }
            }
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
            _nextPitPos.X -= Worldinformation.CarLength * 2;
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
