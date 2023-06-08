using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace NASCAR_Races_Server
{
    [Serializable]
    public class CarMapper : DrawableCar
    {
        public enum STATE
        {
            ON_CIRCUIT,
            OFF_CIRCUIT,
            ON_WAY_TO_PIT_STOP,
            PIT,
            PIT_STOPPED
        }

        public bool IsDisposable;
        public string CarName;
        public float MaxHorsePower;

        public float Speed;

        public float FuelMass;
        public float FuelBurningRatio;

        public float CurrentHorsePower;
        public STATE State;

        public CarMapper() { }
    }
}
