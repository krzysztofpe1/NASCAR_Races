using System;
using System.Collections.Generic;
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
        private int _firstRow;
        private int _secondRow;

        private Thread _thread;
        private bool _killCollisionChecker = false;

        public Worldinformation Worldinformation { get; }
        
        public RaceManager(int straightLength, int turnRadius, int pitPosY, int turnCurveRadius, int penCircuitSize, PictureBox mainPictureBox)
        {
            _canvasWidth = mainPictureBox.Width;
            _canvasHeight = mainPictureBox.Height;
            //_penCircuitSize = penCircuitSize;

            _straightLength = straightLength;
            _turnRadius = turnRadius;
            _pitPosY = pitPosY;
            _turnCurveRadius = turnCurveRadius;
            
            _nextStartingPos = new Point();
            _firstRow = _canvasHeight / 2 + _turnRadius + penCircuitSize / 4;
            _secondRow = _canvasHeight / 2 + _turnRadius - penCircuitSize / 4;
            _nextStartingPos.X = _canvasWidth / 2 + straightLength / 4;
            _nextStartingPos.Y = _firstRow;

            _thread = new(CheckCollisions);

            Worldinformation= new Worldinformation(straightLength, turnRadius, pitPosY, turnCurveRadius, penCircuitSize, 50, mainPictureBox);
        }

        public List<Car> CreateListOfCars(int numberOfCars)
        {
            ListOfCarThreads = new List<CarThread>();
            ListOfCars = new List<Car>();
            for (int i = 0; i < numberOfCars; i++)
            {
                CarThread car = new(NextStartingPoint(), 1000, 70, i.ToString(), Worldinformation);
                ListOfCarThreads.Add(car);
                ListOfCars.Add((Car)car);
            }
            Worldinformation.ListOfCars= ListOfCars;
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
                        if (car2.State != Car.STATE.ON_CIRCUIT && (car2.State != Car.STATE.ON_WAY_TO_PIT_STOP)) continue;
                        if (Math.Abs(car1.X - car2.X) < car1.Length / 2 + car2.Length / 2 && Math.Abs(car1.Y - car2.Y) < car1.Width / 2 + car2.Width / 2)
                        {
                            car1.IsDisposable = true;
                            car2.IsDisposable = true;
                        }
                    }
                }
            }
        }

        private Point NextStartingPoint()
        {
            Point tempPoint = new(_nextStartingPos.X, _nextStartingPos.Y);
            _nextStartingPos.X -= _straightLength / 25;
            _nextStartingPos.Y = (_nextStartingPos.Y == _firstRow) ? _secondRow : _firstRow;
            return tempPoint;
        }

        public void StartRace()
        {
            ListOfCarThreads.ForEach(carThread => { carThread.StartCar(); });
            //_thread.Start();
        }

        public void KillThreads()
        {
            _killCollisionChecker = true;
            ListOfCarThreads.ForEach(carThread => { carThread.Kill(); });
        }
    }
}
