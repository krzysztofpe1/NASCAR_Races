using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class Car
    {
        float weight;
        float fuelCapacity;
        float fuel { get; set; }
        float maxSpeed;
        float speed;
        float headingAngle;
        public float X, Y;

        public float Height;
        public float Width;

        public Car() {
            this.Height = 5.0f;
            this.Width = 10.0f;
        }

        public Car(float x, float y)
        {
            this.X = x;
            this.Y = y;
            this.Height = 5.0f;
            this.Width = 10.0f;
        }

        public Car(float weight, float fuelCapacity, float fuel, float maxSpeed, float speed, float headingAngle, float x, float y)
        {
            this.weight = weight;
            this.fuelCapacity = fuelCapacity;
            this.fuel = fuel;
            this.maxSpeed = maxSpeed;
            this.speed = speed;
            this.headingAngle = headingAngle;
            this.X = x;
            this.Y = y;

            this.Height = 5.0f;
            this.Width = 10.0f;
        }
    }
}
