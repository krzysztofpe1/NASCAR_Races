using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class Worldinformation
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
        public int CarViewingRadius { get; }

        public int x1;
        public int x2;

        public Point LeftPerfectCircle;
        public Point RightPerfectCircle;
        public int PerfectCircleRadius;

        public List<Car> ListOfCars { get; set; }

        public Worldinformation(int straightLength, int turnRadius, int pitPosY, int turnCurveRadius, int penCircuitSize, int carViewingRadius, PictureBox mainPictureBox)
        {
            CanvasWidth = mainPictureBox.Width;
            CanvasHeight = mainPictureBox.Height;
            CanvasCenterX = CanvasWidth / 2;
            CanvasCenterY = CanvasHeight / 2;
            StraightLength = straightLength;
            TurnRadius = turnRadius;
            TurnCurveRadius = turnCurveRadius;
            PitPosY = pitPosY;
            OuterBounds = new Point(CanvasCenterX - StraightLength/2-TurnRadius/2);
            //x1 and x2 are beggining and ending X coordinates of straights
            x1 = CanvasCenterX - straightLength / 2;
            x2 = CanvasCenterX + straightLength / 2;
            List<double> points = Physics.FindCircle(PerfectTurnCirclePoints());
            RightPerfectCircle = new Point((int)points[0], (int)points[1]);
            points= Physics.FindCircle(PerfectTurnCirclePoints(false));
            LeftPerfectCircle = new Point((int)points[0], (int)points[1]);
            PerfectCircleRadius = (int)points[2];

            PenCircuitSize = penCircuitSize;
            CarViewingRadius = carViewingRadius;
        }

        //returns List<Car> that are in CarViewingRadius of callerCar
        public List<Car> NearbyCars(Car callerCar)
        {
            List<Car> res = new List<Car>();
            ListOfCars.ForEach(car =>
            {
                if (callerCar!=car && Math.Pow(callerCar.X - car.X, 2) + Math.Pow(callerCar.Y - car.Y, 2) <= Math.Pow(CarViewingRadius, 2))
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
        public CIRCUIT_PARTS WhatPartOfCircuitIsCarOn(Physics car, bool perfectCircle = true)
        {
            return WhatPartOfCircuitIsCarOn(car.X, car.Y, perfectCircle);
        }
        public CIRCUIT_PARTS WhatPartOfCircuitIsCarOn(float x, float y, bool perfectCircle)
        {
            if (!perfectCircle)
            {
                if (x < x1) return CIRCUIT_PARTS.LEFT_TURN;
                if (x > x2) return CIRCUIT_PARTS.RIGHT_TURN;
            }
            if (x < LeftPerfectCircle.X) return CIRCUIT_PARTS.LEFT_TURN;
            if (x > RightPerfectCircle.X) return CIRCUIT_PARTS.RIGHT_TURN;
            if (y < CanvasCenterY) return CIRCUIT_PARTS.TOP;
            if (y > CanvasCenterY) return CIRCUIT_PARTS.BOTTOM;
            return CIRCUIT_PARTS.PIT;
        }

        public bool IsInBoundsOfCircuit(Car car)
        {
            return IsInBoundsOfCircuit(car.X, car.Y);
        }
        public bool IsInBoundsOfCircuit(float x, float y)
        {
            return false;
        }
        public List<Point> PerfectTurnCirclePoints(bool rightCircle = true)
        {
            List<Point> points = new List<Point>();
            if(rightCircle)
            {
                points.Add(new Point(x2, CanvasCenterY + TurnRadius + PenCircuitSize / 2 - PenCircuitSize / 4));//dol
                points.Add(new Point(x2, CanvasCenterY - TurnRadius - PenCircuitSize / 2 + PenCircuitSize / 4));//gora
                points.Add(new Point(x2 + TurnRadius - PenCircuitSize / 2, CanvasCenterY + PenCircuitSize / 4));//prawo
            }
            else
            {
                points.Add(new Point(x1, CanvasCenterY + TurnRadius + PenCircuitSize / 2 - PenCircuitSize / 4));//dol
                points.Add(new Point(x1, CanvasCenterY - TurnRadius - PenCircuitSize / 2 + PenCircuitSize / 4));//gora
                points.Add(new Point(x1 - TurnRadius + PenCircuitSize / 2, CanvasCenterY - PenCircuitSize / 4));//lewo
            }

            return points;
        }
        public bool IsPointOnCircuit(Point point)
        {
            if (point.Y > CanvasCenterY)
            {
                if (point.Y < CanvasCenterY + TurnRadius + PenCircuitSize / 2) return true;
            }
            return false;
        }
    }
}
