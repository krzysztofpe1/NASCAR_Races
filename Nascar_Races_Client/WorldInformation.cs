using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nascar_Races_Client
{
    public class WorldInformation
    {
        public int CanvasWidth { get; }
        public int CanvasHeight { get; }
        public int CanvasCenterX { get; }
        public int CanvasCenterY { get; }
        public int StraightLength { get; }
        public int TurnRadius { get; }
        public int TurnCurveRadius { get; }
        public int PitPosY { get; }
        public Point OuterBounds { get; }
        public Point InnerBounds { get; }
        public int PenCircuitSize { get; }
        public int PenCarSize { get; }

        public int NumberOfCars { get; } = 10;

        public int CarViewingRadius { get; }
        public int CarsSafeDistance { get; } = 10;
        public int CarPitStopEntryOffset { get; private set; }
        public int CarMaxSpeedInPit { get; } = 40;
        public int CarInitialFuelMass { get; } = 40;
        public int CarLength { get; } = 15;
        public int CarWidth { get; } = 10;

        public int CarLengthOfPittingManouver { get; private set; }
        public int CarWidthOfPittingManouver { get; set; }

        public int x1;
        public int x2;

        public List<Car> ListOfCars { get; set; }

        public WorldInformation(int straightLength, int turnRadius, int pitPosY, int turnCurveRadius, int penCircuitSize, int penCarSize, int carViewingRadius, PictureBox mainPictureBox)
        {
            CanvasWidth = mainPictureBox.Width;
            CanvasHeight = mainPictureBox.Height;
            CanvasCenterX = CanvasWidth / 2;
            CanvasCenterY = CanvasHeight / 2;
            StraightLength = straightLength;
            TurnRadius = turnRadius;
            TurnCurveRadius = turnCurveRadius;
            PitPosY = pitPosY;
            OuterBounds = new Point(CanvasCenterX - StraightLength / 2 - TurnRadius / 2);
            //x1 and x2 are beggining and ending X coordinates of straights
            x1 = CanvasCenterX - straightLength / 2;
            x2 = CanvasCenterX + straightLength / 2;

            PenCircuitSize = penCircuitSize;
            PenCarSize = penCarSize;
            CarViewingRadius = carViewingRadius;

            CarPitStopEntryOffset = PenCircuitSize / 3;
            CarLengthOfPittingManouver = CarLength * 3;
        }

        //returns List<Car> that are in CarViewingRadius of callerCar
        public List<Car> NearbyCars(Car callerCar)
        {
            return new List<Car>(); //TODO zmienic to
            List<Car> res = new List<Car>();
            ListOfCars.ForEach(car =>
            {
                if (callerCar != car && Math.Pow(callerCar.X - car.X, 2) + Math.Pow(callerCar.Y - car.Y, 2) <= Math.Pow(CarViewingRadius, 2) && car.IsDisposable != true)
                {
                    res.Add(car);
                }
            });
            return res;
        }
        public enum CIRCUIT_PARTS
        {
            LEFT_TURN,
            RIGHT_TURN,
            TOP,
            BOTTOM,
            PIT
        }
        public CIRCUIT_PARTS WhatPartOfCircuitIsCarOn(Physics car)
        {
            return WhatPartOfCircuitIsCarOn(car.X, car.Y);
        }
        public CIRCUIT_PARTS WhatPartOfCircuitIsCarOn(float x, float y)
        {
            if (x <= x1) return CIRCUIT_PARTS.LEFT_TURN;
            if (x >= x2) return CIRCUIT_PARTS.RIGHT_TURN;
            if (y < CanvasCenterY) return CIRCUIT_PARTS.TOP;
            if (y > CanvasCenterY) return CIRCUIT_PARTS.BOTTOM;
            return CIRCUIT_PARTS.PIT;
        }
        public float DistanceToEdgeOfTrack(Physics car, bool outerEdge = true)
        {
            var part = WhatPartOfCircuitIsCarOn(car);
            if (part == CIRCUIT_PARTS.TOP)
            {
                float trackY = CanvasCenterY - TurnRadius;
                if (outerEdge) trackY -= PenCircuitSize / 2;
                else trackY += PenCircuitSize / 2;
                return Math.Abs(car.Y - trackY);
            }
            if (part == CIRCUIT_PARTS.BOTTOM)
            {
                float trackY = CanvasCenterY + TurnRadius;
                if (outerEdge) trackY += PenCircuitSize / 2;
                else trackY -= PenCircuitSize / 2;
                return Math.Abs(trackY - car.Y);
            }
            return 0;
        }
    }
}
