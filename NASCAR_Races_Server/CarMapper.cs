using NASCAR_Races;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace NASCAR_Races_Server
{
    public class CarMapper
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
        public bool Started;
        public string CarName;
        public float MaxHorsePower;



        public float X;
        public float Y;
        public float Length;
        public float Width;
        public float Speed;
        public float HeadingAngle;
        public float _currentAcceleration;
        public float _mass;
        public float _frictionofweels;
        public System.DateTime _lastExecutionTime;

        public Point _leftCircle;
        public Point _rightCircle;
        public int _circleRadius;
        public Point _pitPos;

        public float FuelMass;
        public float FuelBurningRatio;

        public float CurrentHorsePower;

        public List<Car> _neighbouringCars;

        public bool _recalculateHeadingAngle;

        public double currentTurnAngle;

        public Worldinformation _worldInf;
        public int _carSafeDistance;
        public STATE State;

        public CarMapper() { }
        public Car CrateNewCar()// Tak jak mówiłeś tymczasowo tak jest
        {
            var car = new Car(this.X, this.Y, this._mass,this.CarName, this.MaxHorsePower, this._worldInf);
            car.unMapPhysics(this);
            return car;
        }
    }
}
