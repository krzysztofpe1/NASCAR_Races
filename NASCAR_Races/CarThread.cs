using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class CarThread : Car
    {
        private Thread _thread;
        public CarThread() { }
        public CarThread(Car car) { }
        public CarThread(float x, float y) : base(x, y) { }
        public CarThread(Point point, float weight, float fuelCapacity) : base(point.X, point.Y, weight, fuelCapacity)
        {
            _thread = new(this.Move);
        }
        public CarThread(float x, float y, float weight, float fuelCapacity) : base(x, y, weight, fuelCapacity)
        {
            _thread = new(this.Move);
        }

        public void StartCar()
        {
            _thread.Start();
        }
        public void Kill()
        {
            IsDisposable = true;
        }
    }
}
