using NASCAR_Races_Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nascar_Races_Client
{
    internal class Painter
    {
        private int _canvasWidth;
        private int _canvasHeight;

        private int _straightLength;
        private int _turnRadius;
        private int _pitPosY;
        private Pen _penCircuit;
        private Pen _penPit;
        private Pen _penCar;
        private int _halfPaintBrushSize = 30;
        //Points of start and end of the straights
        public List<DrawableCar> listOfCars { get; set; }
        private int _x1, _x2;

        private WorldInformation _worldinformation;
        public Painter(WorldInformation worldinformation)
        {
            _canvasHeight = worldinformation.CanvasHeight;
            _canvasWidth = worldinformation.CanvasWidth;
            _straightLength = worldinformation.StraightLength;
            _turnRadius = worldinformation.TurnRadius;
            _pitPosY = worldinformation.PitPosY;
            _penCircuit = new Pen(Color.Black, worldinformation.PenCircuitSize + 7);
            _penPit = new Pen(Color.Orange, worldinformation.PenCircuitSize / 2);
            _penCar = new Pen(Color.Red, worldinformation.PenCarSize);
            _x1 = _canvasWidth / 2 - _straightLength / 2;
            _x2 = _canvasWidth / 2 + _straightLength / 2;
            listOfCars = new();
        }
        public void PaintCircuit(Graphics g)
        {

            //Pen temp1 = new Pen(Color.Orange, _halfPaintBrushSize * 2);
            //Pen temp2 = new Pen(Color.Green, _halfPaintBrushSize * 2);

            //Painting Pit Stop
            g.DrawLine(_penPit, _x1, _pitPosY, _x2 + 1, _pitPosY);
            int startX = _x1 - _turnRadius + _halfPaintBrushSize / 2, startY = _canvasHeight / 2 - _pitPosY / 2;
            int endX = _x1, endY = _pitPosY;
            g.DrawArc(_penPit, startX, startY, (endX - startX) * 2, (endY - startY), 90, 90);
            startX = _x2;
            endX = _x2 + _turnRadius - _halfPaintBrushSize / 2;
            g.DrawArc(_penPit, 2 * startX - endX, startY, (endX - startX) * 2, (endY - startY), 0, 90);
            //Painting Circuit
            g.DrawArc(_penCircuit, _x1 - _turnRadius, _canvasHeight / 2 - _turnRadius, _turnRadius * 2, _turnRadius * 2, 90, 180);
            g.DrawArc(_penCircuit, _x2 - _turnRadius, _canvasHeight / 2 - _turnRadius, _turnRadius * 2, _turnRadius * 2, 270, 180);
            g.DrawLine(_penCircuit, _x1, _canvasHeight / 2 - _turnRadius, _x2 + 1, _canvasHeight / 2 - _turnRadius);
            g.DrawLine(_penCircuit, _x1, _canvasHeight / 2 + _turnRadius, _x2 + 1, _canvasHeight / 2 + _turnRadius);
        }

        public void PaintCarsPosition(Graphics g)
        {
            if (listOfCars.Count() == 0)
            {
                Debug.WriteLine("Pusta lista");
                return;
            }
            Debug.WriteLine("lista niepusta");
            listOfCars.ForEach(car => { PaintCar(g, car); });
            //Console.Clear();
            //listOfCars.ForEach(car => { WriteLogs(car); });
        }

        private void PaintCar(Graphics g, DrawableCar car)
        {
            if (!float.IsNaN(car.X))
            {
                g.TranslateTransform(car.X, car.Y);
                g.RotateTransform(car.HeadingAngle);
                g.TranslateTransform(-car.X, -car.Y);
                g.DrawRectangle(_penCar, car.X - car.Length / 2, car.Y - car.Width / 2, car.Length, car.Width);
                g.ResetTransform();
            }
        }
    }
}
