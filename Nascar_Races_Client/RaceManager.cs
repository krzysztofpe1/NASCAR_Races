using NASCAR_Races_Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nascar_Races_Client
{
    internal class RaceManager
    {
        public Car MyCar { get; private set; }
        private ClientTCPHandler _client;
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
            _client = new ClientTCPHandler(WorldInformation, NextStartingPoint(), NextPitPoint());
        }
        public List<Car> getCars()
        {
            var temp = new List<Car>();
            temp.Add(_client.MyCar);
            return temp;
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
            _nextPitPos.X -= WorldInformation.CarLength * 4;
            return temp;
        }

        public void StartRace()
        {
            //TODO przenies to gdzies
            _client.StartCar();
        }
        public void PrepareRace()
        {
            //MyCar = new Car();
        }
        public void EndRace()
        {

        }
        public void Dispose()
        {
            _client.Dispose();
        }

    }
}
