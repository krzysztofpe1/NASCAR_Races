using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class Car : CarController
    {
        public enum STATE
        {
            ON_CIRCUIT,
            OFF_CIRCUIT,
            ON_WAY_TO_PIT_STOP,
            PIT_STOP
        }

        private int _canvasWidth;
        private int _canvasHeight;
        
        public STATE State { get; private set; } = Car.STATE.ON_CIRCUIT;

        public bool IsDisposable { get; set; } = false;

        private float _weight;
        
        private float _fuel;
        private float _fuelCapacity;

        private float _maxSpeed;
        private float _acceleration;

        private Worldinformation _world;

        public Car() {}
        public Car(float x, float y)
        {
            X = x;
            Y = y;
        }
        //TODO
        //zaimplementowac klase swiat z wszystkimi wymiarami
        public Car(float x, float y, float weight, float fuelCapacity, Worldinformation worldInfo)
        {
            X = x;
            Y = y;
            _weight = weight;
            _fuelCapacity = fuelCapacity;
            _fuel = fuelCapacity;
            _world = worldInfo;
        }

        Random random = new();
        public void Move()
        {
            while (!IsDisposable)
            {
                
            }
        }


    }
}
