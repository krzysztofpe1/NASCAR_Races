using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class Car
    {
        public float weight;
        public float headingAngle;
        public float x, y;
        public Engine engine;

        public float height = 5.0f;
        public float width = 10.0f;

        public Car() {}

        public Car(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Car(float x, float y, float weight, float headingAngle,Engine engine)
        {
            this.x = x;
            this.y = y;
            this.weight = weight;
            this.headingAngle = headingAngle;
            this.engine = engine;
        }
    }
}
