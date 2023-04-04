using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class Car
    {
        public float weight { get; set; }
        public float headingAngle;
        public float x { get; set; }
        public float y { get; set; }

        public float length = 10.0f;
        public float width = 5.0f;
        public float fuelCapacity;
        public float fuel { get; set; }
        public float maxSpeed;
        public float speed;
        public float acceleration;

        public Car() {}

        public Car(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Car(float x, float y, float weight, float headingAngle)
        {
            this.x = x;
            this.y = y;
            this.weight = weight;
            this.headingAngle = headingAngle;
        }

        public void Move()
        {
            /*// Bot
            if (car.x < X_MAX && car.x > X_MIN && car.y > Y_MID) car.x += Physics.Street(car.engine.speed);

            // Top
            if (car.x < X_MAX && car.x > X_MIN && car.y < Y_MID) car.x -= Physics.Street(car.engine.speed);

            // Right
            if (car.x >= X_MAX)
            {
                if (car.y > Y_MID) car.x += Physics.Street(car.engine.speed);
                else car.x -= Physics.Street(car.engine.speed);

                car.y -= Physics.Street(car.engine.speed);
            }

            // Left
            if (car.x <= X_MIN)
            {
                if (car.y > Y_MID) car.x += Physics.Street(car.engine.speed);
                else car.x -= Physics.Street(car.engine.speed);

                car.y += Physics.Street(car.engine.speed);
            }*/
        }
    }
}
