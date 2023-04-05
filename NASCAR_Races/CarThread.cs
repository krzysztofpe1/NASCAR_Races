using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class CarThread : Car
    {
        Thread thread;
        public CarThread() { }
        public CarThread(Car car) { }
        public CarThread(float x, float y) : base(x, y) { }
        public CarThread(float x, float y, float weight, float fuelCapacity) : base(x, y, weight, fuelCapacity)
        {
            thread = new(this.Move);
        }
        public void StartCar()
        {
            thread.Start();
        }


    }
}
