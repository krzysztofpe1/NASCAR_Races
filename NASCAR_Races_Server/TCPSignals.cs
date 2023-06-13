using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races_Server
{
    public class TCPSignals
    {
        public const int startRaceSignal = 42;
        public const int killCarSignal = 43;
        public const int endRaceSignal = 44;
        public const int serverReadyForData = 45;
        public const int clientReadyForData = 46;
    }
}
