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
            //Pen temp1 = new Pen(Color.Orange, halfPaintBrushSize * 2);
            //Pen temp2 = new Pen(Color.Green, halfPaintBrushSize * 2);

            //Points of beggining and end of the straights
            int x1 = canvasWidth / 2 - straightLength / 2;
            int x2 = canvasWidth / 2 + straightLength / 2;
            //Painting Pit Stop
            g.DrawLine(penPit, x1, pitPosY, x2+1, pitPosY);
            int startX = x1 - turnRadius + halfPaintBrushSize / 2, startY = canvasHeight / 2 - pitPosY / 2;
            int endX = x1, endY = pitPosY;
            g.DrawArc(penPit, startX, startY, (endX - startX) * 2, (endY - startY), 90, 90);
            startX=x2;
            endX = x2 + turnRadius - halfPaintBrushSize / 2;
            g.DrawArc(penPit, 2*startX-endX, startY, (endX - startX) * 2, (endY - startY), 0, 90);
            //Painting Circuit
            g.DrawArc(penCircuit, x1 - turnRadius, canvasHeight / 2 - turnRadius, turnRadius * 2, turnRadius * 2, 90, 180);
            g.DrawArc(penCircuit, x2 - turnRadius, canvasHeight / 2 - turnRadius, turnRadius * 2, turnRadius * 2, 270, 180);
            g.DrawLine(penCircuit, x1, canvasHeight / 2 - turnRadius, x2 + 1, canvasHeight / 2 - turnRadius);
            g.DrawLine(penCircuit, x1, canvasHeight / 2 + turnRadius, x2 + 1, canvasHeight / 2 + turnRadius);
        }
    }
}
