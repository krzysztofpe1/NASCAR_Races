using System;
using System.Collections.Generic;
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
        private int _penCircuitSize;

        private int _straightLength;
        private int _turnRadius;
        private int _pitPosY;
        private int _turnCurveRadius;

        private Point _nextStartingPos;
        private int _firstRow;
        private int _secondRow;
        
        public RaceManager(int straightLength, int turnRadius, int pitPosY, int turnCurveRadius, int penCircuitSize, PictureBox mainPictureBox)
        {
            _canvasWidth = mainPictureBox.Width;
            _canvasHeight = mainPictureBox.Height;
            _penCircuitSize = penCircuitSize;

            _straightLength = straightLength;
            _turnRadius = turnRadius;
            _pitPosY = pitPosY;
            _turnCurveRadius = turnCurveRadius;
            
            _nextStartingPos = new Point();
            _firstRow = _canvasHeight / 2 + _turnRadius + penCircuitSize / 4;
            _secondRow = _canvasHeight / 2 + _turnRadius - penCircuitSize / 4;
            _nextStartingPos.X = _canvasWidth / 2 + straightLength / 4;
            _nextStartingPos.Y = _firstRow;
        }

        public List<Car> CreateListOfCars(int numberOfCars)
        {
            ListOfCarThreads = new List<CarThread>();
            ListOfCars = new List<Car>();
            for (int i = 0; i < numberOfCars; i++)
            {
                CarThread car = new(NextStartingPoint(), 1000, 70);
                ListOfCarThreads.Add(car);
                ListOfCars.Add((Car)car);
            }
            return ListOfCars;
        }

        private Point NextStartingPoint()
        {
            Point tempPoint = new(_nextStartingPos.X, _nextStartingPos.Y);
            _nextStartingPos.X -= _straightLength / 30;
            _nextStartingPos.Y = (_nextStartingPos.Y == _firstRow) ? _secondRow : _firstRow;
            return tempPoint;
        }

        public void StartRace()
        {
            ListOfCarThreads.ForEach(carThread => { carThread.StartCar(); });
        }
    }
}
