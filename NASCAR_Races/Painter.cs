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
        int _turnRadius;
        int pitPosY;
        Pen penCircuit;
        Pen penPit;
        Pen penCar;
        int halfPaintBrushSize = 30;
        //Points of start and end of the straights
        public List<Car> listOfCars { set; get; }
        int x1, x2;
        public Painter(int canvasWidth, int canvasHeight, int straightLength, int _turnRadius, int pitPosY)
        {
            this.canvasWidth = canvasWidth;
            this.canvasHeight = canvasHeight;
            this.straightLength = straightLength;
            this._turnRadius = _turnRadius;
            this.pitPosY = pitPosY;

            halfPaintBrushSize = 30;
            penCircuit = new Pen(Color.Black, halfPaintBrushSize * 2);
            penPit = new Pen(Color.Orange, halfPaintBrushSize);
            penCar = new Pen(Color.Red, 5);//size of paint brush for car
            x1 = canvasWidth / 2 - straightLength / 2;
            x2 = canvasWidth / 2 + straightLength / 2;
        }
        public void PaintCircuit(Graphics g)
        {
            
            //Pen temp1 = new Pen(Color.Orange, halfPaintBrushSize * 2);
            //Pen temp2 = new Pen(Color.Green, halfPaintBrushSize * 2);
            
            //Painting Pit Stop
            g.DrawLine(penPit, x1, pitPosY, x2+1, pitPosY);
            int startX = x1 - _turnRadius + halfPaintBrushSize / 2, startY = canvasHeight / 2 - pitPosY / 2;
            int endX = x1, endY = pitPosY;
            g.DrawArc(penPit, startX, startY, (endX - startX) * 2, (endY - startY), 90, 90);
            startX=x2;
            endX = x2 + _turnRadius - halfPaintBrushSize / 2;
            g.DrawArc(penPit, 2*startX-endX, startY, (endX - startX) * 2, (endY - startY), 0, 90);
            //Painting Circuit
            g.DrawArc(penCircuit, x1 - _turnRadius, canvasHeight / 2 - _turnRadius, _turnRadius * 2, _turnRadius * 2, 90, 180);
            g.DrawArc(penCircuit, x2 - _turnRadius, canvasHeight / 2 - _turnRadius, _turnRadius * 2, _turnRadius * 2, 270, 180);
            g.DrawLine(penCircuit, x1, canvasHeight / 2 - _turnRadius, x2 + 1, canvasHeight / 2 - _turnRadius);
            g.DrawLine(penCircuit, x1, canvasHeight / 2 + _turnRadius, x2 + 1, canvasHeight / 2 + _turnRadius);
        }

        internal void PaintCarsPosition(Graphics g)
        {
            if (listOfCars.Count() == 0) throw new InvalidOperationException("List of cars is empty");
            foreach (Car car in listOfCars) g.DrawRectangle(penCar, car.X, car.Y, car.Length, car.Width);
        }
        
    }
}
