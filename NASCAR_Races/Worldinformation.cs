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
        public Worldinformation(int straightLength, int turnRadius, int pitPosY, int turnCurveRadius, PictureBox mainPictureBox)
        {
            CanvasWidth = mainPictureBox.Width;
            CanvasHeight = mainPictureBox.Height;
            StraightLength = straightLength;
            TurnRadius = turnRadius;
            TurnCurveRadius = turnCurveRadius;
            PitPosY = pitPosY;
        }

        public void IsInBoundsOfCircuit(Car car)
        {

        }
    }
}
