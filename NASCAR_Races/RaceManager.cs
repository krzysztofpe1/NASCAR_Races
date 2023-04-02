using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class RaceManager
    {
        int straightLength;
        int turnRadius;
        int pitPosY;
        int turnCurveRadius;
        public RaceManager(int straightLength, int turnRadius, int pitPosY, int turnCurveRadius)
        {
            this.straightLength = straightLength;
            this.turnRadius = turnRadius;
            this.pitPosY = pitPosY;
            this.turnCurveRadius = turnCurveRadius;
        }


    }
}
