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
        public int x1;
        public int x2;

        public Point LeftCircle;
        public Point RightCircle;

        public Worldinformation(PictureBox mainPictureBox)
        {
            CanvasWidth = mainPictureBox.Width;
            CanvasHeight = mainPictureBox.Height;
        }
        public Worldinformation(int straightLength, int turnRadius, int pitPosY, int turnCurveRadius, PictureBox mainPictureBox)
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
        }

        public bool IsInBoundsOfCircuit(Car car)
        {
            return IsInBoundsOfCircuit(car.X, car.Y);
        }
        public bool IsInBoundsOfCircuit(float x, float y)
        {
            return false;
        }
    }
}
