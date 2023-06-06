using NASCAR_Races_Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nascar_Races_Client
{
    internal class RaceManager
    {
        public Car MyCar { get; private set; }
        public List<CarClientHandler> ListOfCarThreads;
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

        public WorldInformation WorldInformation { get; }
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


            WorldInformation = new WorldInformation(straightLength, turnRadius, pitPosY, turnCurveRadius, penCircuitSize, penCarSize, 100, mainPictureBox);

            _nextPitPos = new Point();
            _nextPitPos.Y = pitPosY - penCircuitSize / 4 + WorldInformation.CarWidth / 2;
            WorldInformation.CarWidthOfPittingManouver = pitPosY - _nextPitPos.Y;
            _nextPitPos.X = WorldInformation.x2 - WorldInformation.CarLength;
        }

        public void StartRace()
        {

        }
        public void PrepareRace()
        {
            //MyCar = new Car();
        }
        public void EndRace()
        {

        }

    }
}
