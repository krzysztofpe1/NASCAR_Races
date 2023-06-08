using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races_Server
{
    [Serializable]
    public abstract class DrawableCar
    {
        public float X;
        public float Y;
        public float HeadingAngle;
        public float Length;
        public float Width;
    }
}
