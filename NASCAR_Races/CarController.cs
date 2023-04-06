using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class CarController
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Length { get; private set; } = 15;
        public float Width { get; private set; } = 10;
        public float Speed { get; private set; }
        public float HeadingAngle { get; private set; } = 0;

    }
}
