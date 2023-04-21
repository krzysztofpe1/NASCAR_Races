using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class Car : Physics
    {
        public enum STATE
        {
            ON_CIRCUIT,
            OFF_CIRCUIT,
            ON_WAY_TO_PIT_STOP,
            PIT_STOP
        }
        
        public STATE State { get; private set; } = Car.STATE.ON_CIRCUIT;

        public bool IsDisposable { get; set; } = false;

        
        public float Length { get; private set; } = 15;
        public float Width { get; private set; } = 10;
        private List<Car> _neighbouringCars;
        
        

        private Worldinformation _worldInfo;
        //TODO
        //zaimplementowac klase swiat z wszystkimi wymiarami
        public Car(float x, float y, float weight, float fuelCapacity, Worldinformation worldInfo):base(x,y,weight,fuelCapacity,0.3f, worldInfo)
        {
            _worldInfo = worldInfo;
        }
        Random random = new Random();
        public void Move()
        {
            _neighbouringCars = _worldInfo.NearbyCars(this);
            int counter = 0;
            while (!IsDisposable)
            {
                //refreshing neighbouring cars list every 10 iterations
                if (++counter >= 10)
                {
                    counter = 0;
                    _neighbouringCars = _worldInfo.NearbyCars(this);
                }
                RunPhysic();
                Thread.Sleep(10);
            }
        }


    }
}
