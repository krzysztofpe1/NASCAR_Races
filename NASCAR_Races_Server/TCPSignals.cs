using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races_Server
{
    public class TCPSignals
    {
        public const int startRaceSignal = 420;
        public const int killCarSignal = 421;
        public const int endRaceSignal = 422;
    }
}
