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
        public CarThread(Point point, Point pitPos, float weight, string carName, float maxHorsePower, Worldinformation worldinformation) : base(point.X, point.Y, weight, carName, maxHorsePower, worldinformation)
        {
            _pitPos = pitPos;
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
