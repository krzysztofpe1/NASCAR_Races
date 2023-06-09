using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace NASCAR_Races_Server
{
    [Serializable]
    [DataContract]
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
        [DataMember]
        public bool IsDisposable;
        [DataMember]
        public string CarName;
        [DataMember]
        public float MaxHorsePower;

        [DataMember]
        public float Speed;

        [DataMember]
        public float FuelMass;
        [DataMember]
        public float FuelBurningRatio;

        [DataMember]
        public float CurrentHorsePower;
        [DataMember]
        public STATE State;

        public CarMapper() { }
    }
}
