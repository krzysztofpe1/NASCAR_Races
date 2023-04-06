using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class Car
    {
        public enum STATE
        {
            ON_CIRCUIT,
            OFF_CIRCUIT,
            ON_WAY_TO_PIT_STOP,
            PIT_STOP
        }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Length { get; private set; } = 15;
        public float Width { get; private set; } = 10;
        public float Speed { get; private set; }
        public float HeadingAngle { get; private set; } = 0;
        public STATE State { get; private set; } = Car.STATE.ON_CIRCUIT;

        public bool IsDisposable { get; set; } = false;

        private float _weight;
        
        private float _fuel;
        private float _fuelCapacity;

        private float _horsePower;
        private float _maxSpeed;
        private float _acceleration;

        public Car() {}
        public Car(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Car(float x, float y, float weight, float fuelCapacity)
        {
            X = x;
            Y = y;
            _weight = weight;
            _fuelCapacity = fuelCapacity;
            _fuel = fuelCapacity;
        }

        Random random = new();
        public void Move()
        {
            while (!IsDisposable)
            {
                if (random.Next(2) == 0)
                {
                    X++;
                    HeadingAngle++;
                }
                Thread.Sleep(1);
            }
        }


    }
}
