using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class Painter
    {
        private int _canvasWidth, _canvasHeight;

        private int _straightLength;
        private int _turnRadius;
        private int _pitPosY;
        private Pen _penCircuit;
        private Pen _penPit;
        private Pen _penCar;
        private int _halfPaintBrushSize = 30;
        //Points of start and end of the straights
        public List<Car> listOfCars { set; get; }
        private int _x1, _x2;
        public Painter(int canvasWidth, int canvasHeight, int straightLength, int turnRadius, int pitPosY)
        {
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;
            _straightLength = straightLength;
            _turnRadius = turnRadius;
            _pitPosY = pitPosY;

            _halfPaintBrushSize = 30;
            _penCircuit = new Pen(Color.Black, _halfPaintBrushSize * 2);
            _penPit = new Pen(Color.Orange, _halfPaintBrushSize);
            _penCar = new Pen(Color.Red, 5);//size of paint brush for car
            _x1 = _canvasWidth / 2 - _straightLength / 2;
            _x2 = _canvasWidth / 2 + _straightLength / 2;
        }
        public void PaintCircuit(Graphics g)
        {
            
            //Pen temp1 = new Pen(Color.Orange, _halfPaintBrushSize * 2);
            //Pen temp2 = new Pen(Color.Green, _halfPaintBrushSize * 2);
            
            //Painting Pit Stop
            g.DrawLine(_penPit, _x1, _pitPosY, _x2+1, _pitPosY);
            int startX = _x1 - _turnRadius + _halfPaintBrushSize / 2, startY = _canvasHeight / 2 - _pitPosY / 2;
            int endX = _x1, endY = _pitPosY;
            g.DrawArc(_penPit, startX, startY, (endX - startX) * 2, (endY - startY), 90, 90);
            startX=_x2;
            endX = _x2 + _turnRadius - _halfPaintBrushSize / 2;
            g.DrawArc(_penPit, 2*startX-endX, startY, (endX - startX) * 2, (endY - startY), 0, 90);
            //Painting Circuit
            g.DrawArc(_penCircuit, _x1 - _turnRadius, _canvasHeight / 2 - _turnRadius, _turnRadius * 2, _turnRadius * 2, 90, 180);
            g.DrawArc(_penCircuit, _x2 - _turnRadius, _canvasHeight / 2 - _turnRadius, _turnRadius * 2, _turnRadius * 2, 270, 180);
            g.DrawLine(_penCircuit, _x1, _canvasHeight / 2 - _turnRadius, _x2 + 1, _canvasHeight / 2 - _turnRadius);
            g.DrawLine(_penCircuit, _x1, _canvasHeight / 2 + _turnRadius, _x2 + 1, _canvasHeight / 2 + _turnRadius);
        }

        internal void PaintCarsPosition(Graphics g)
        {
            if (listOfCars.Count() == 0) throw new InvalidOperationException("List of cars is empty");
            listOfCars.ForEach(car => { g.DrawRectangle(_penCar, car.X, car.Y, car.Length, car.Width); });
        }
        
    }
}
