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
        public CarThread(Point point, float weight, float fuelCapacity, string carName, Worldinformation worldinformation) : base(point.X, point.Y, weight, fuelCapacity, carName, worldinformation)
        {
            _thread = new(this.Move);
            _thread.Start();
        }
        public CarThread(float x, float y, float weight, float fuelCapacity, string carName, Worldinformation worldinformation) : base(x, y, weight, fuelCapacity, carName, worldinformation)
        {
            _thread = new(this.Move);
            _thread.Start();
        }

        public void StartCar()
        {
            Started = true;
        }
        public void Kill()
        {
            IsDisposable = true;
        }
    }
}
