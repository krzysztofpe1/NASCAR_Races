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
        float headingAngle;
        public float x, y;
        Engine engine;

        public float height = 5.0f;
        public float width = 10.0f;

        public Car() {}

        public Car(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Car(float weight, float headingAngle, float x, float y, Engine engine)
        {
            this.weight = weight;
            this.headingAngle = headingAngle;
            this.x = x;
            this.y = y;
            this.engine = engine;
        }
    }
}
