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
        public int StraightLength { get; }
        public int TurnRadius { get; }
        public int TurnCurveRadius { get; }
        public int PitPosY { get; }
        public Point OuterBounds { get; }
        public Point InnerBounds { get; }
        public int PenCircuitSize { get; }

        public int x1;
        public int x2;

        public Point LeftCircle;
        public Point RightCircle;

        public Worldinformation(PictureBox mainPictureBox)
        {
            CanvasWidth = mainPictureBox.Width;
            CanvasHeight = mainPictureBox.Height;
        }
        public Worldinformation(int straightLength, int turnRadius, int pitPosY, int turnCurveRadius, int penCircuitSize, PictureBox mainPictureBox)
        {
            CanvasWidth = mainPictureBox.Width;
            CanvasHeight = mainPictureBox.Height;
            StraightLength = straightLength;
            TurnRadius = turnRadius;
            TurnCurveRadius = turnCurveRadius;
            PitPosY = pitPosY;
            OuterBounds = new Point(CanvasWidth/2-StraightLength/2-TurnRadius/2);
            x1 = CanvasWidth / 2 - straightLength / 2;
            x2 = CanvasWidth / 2 + straightLength / 2;
            LeftCircle = new Point(x1, CanvasHeight / 2);
            RightCircle = new Point(x2, CanvasHeight / 2);
            PenCircuitSize = penCircuitSize;
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
                points.Add(new Point(x2, CanvasHeight / 2 + TurnRadius + PenCircuitSize / 2 - PenCircuitSize / 4));//dol
                points.Add(new Point(x2, CanvasHeight / 2 - TurnRadius - PenCircuitSize / 2 + PenCircuitSize / 4));//gora
                points.Add(new Point(x2 + TurnRadius - PenCircuitSize / 2, CanvasHeight / 2 + PenCircuitSize / 4));//prawo
            }
            else
            {
                points.Add(new Point(x1, CanvasHeight / 2 + TurnRadius + PenCircuitSize / 2 - PenCircuitSize / 4));//dol
                points.Add(new Point(x1, CanvasHeight / 2 - TurnRadius - PenCircuitSize / 2 + PenCircuitSize / 4));//gora
                points.Add(new Point(x1 - TurnRadius + PenCircuitSize / 2, CanvasHeight / 2 - PenCircuitSize / 4));//lewo
            }

            return points;
        }
    }
}
