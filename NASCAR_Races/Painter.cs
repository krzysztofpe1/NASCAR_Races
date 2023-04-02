using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class Painter
    {
        int canvasWidth, canvasHeight;
        int straightLength;
        int turnRadius;
        int pitPosY;
        public Painter(int canvasWidth, int canvasHeight, int straightLength, int turnRadius, int pitPosY)
        {
            this.canvasWidth = canvasWidth;
            this.canvasHeight = canvasHeight;
            this.straightLength = straightLength;
            this.turnRadius = turnRadius;
            this.pitPosY = pitPosY;
        }
        public void PaintCircuit(Graphics g)
        {
            int halfPaintBrushSize=30;
            Pen penCircuit = new Pen(Color.Black, halfPaintBrushSize*2);
            Pen penPit = new Pen(Color.Orange, halfPaintBrushSize);
            Pen temp1 = new Pen(Color.Orange, halfPaintBrushSize * 2);
            Pen temp2 = new Pen(Color.Green, halfPaintBrushSize * 2);

            //Painting Circuit
            /*g.DrawArc(penCircuit, (int)(1.0 / 6.0 * maxX - r + halfPaintBrushSize), maxY / 2 - r, turnRadius * 2, turnRadius * 2, 90, 180);
            
            g.DrawArc(penCircuit, (int)(5.0 / 6.0 * maxX - r - halfPaintBrushSize), maxY / 2 - r, turnRadius * 2, turnRadius * 2, 270, 180);*/
            g.DrawArc(temp1, canvasWidth / 2 - straightLength / 2 - turnRadius, canvasHeight / 2 - turnRadius, turnRadius * 2, turnRadius * 2, 90, 180);
            g.DrawArc(temp2, canvasWidth / 2 + straightLength / 2 - turnRadius, canvasHeight / 2 - turnRadius, turnRadius * 2, turnRadius * 2, 270, 180);
            g.DrawLine(penCircuit, (canvasWidth / 2 - straightLength / 2), canvasHeight / 2 - turnRadius, (canvasWidth / 2 + straightLength / 2 + 1), canvasHeight / 2 - turnRadius);
            g.DrawLine(penCircuit, (canvasWidth / 2 - straightLength / 2), canvasHeight / 2 + turnRadius, (canvasWidth / 2 + straightLength / 2 + 1), canvasHeight / 2 + turnRadius);
            //Painting Pit Stop
            //int pitStopLength = maxX / 3;
            //g.DrawLine(penPit, (maxX - pitStopLength) / 2, (maxY + r) / 2, (maxX + pitStopLength) / 2, (maxY + r) / 2);
            /*g.DrawLine(penPit, (maxX- pitStopLength)/2, (maxY+r)/2, (maxX + pitStopLength) / 2, (maxY + r) / 2);
            Point[] controlPoints = new Point[]
            {
                new Point((int)(1.0 / 6.0 * maxX), maxY / 2),  // punkt startowy
                new Point((int)((1.0 / 6.0 * maxX)+(maxX- pitStopLength)/2)/2, maxY / 2 + r/4), // punkt pośredni
                new Point((maxX- pitStopLength)/2, (maxY+r)/2)  // punkt końcowy
            };
            g.DrawCurve(penPit, controlPoints, tension: 0.5f);*/
        }
    }
}
