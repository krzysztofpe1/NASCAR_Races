using System;
using System.Runtime.InteropServices;

namespace NASCAR_Races
{
    public class Physics
    {
        const float accelerationOfGravity = 9.81f;
        const float trackAngle = 10;
        const float airDensity = 1.225f;
        const float frontSurface = 2.5f;
        const float carAirDynamic = 0.35f;
        const float RadiusOfTurn = 90;


        private float _currentAcceleration;
        private float _mass;
        private float _currentHorsePower = 4000; // Force should be in the Future
        private float _radiusofCar;
        private float _radiusofWells;
        private float _frictionofweels;
        private float _radiusCar = 0;
        private float _fuelMass;
        private System.DateTime _lastExecutionTime;

        protected float FuelBurningRatio = 0.5f;
        protected float Speed = 0;


        public Physics() { }
        public Physics(float Basemass, float frictionofweels, float fuelMass)
        {
            this._lastExecutionTime = DateTime.Now;
            this._fuelMass = fuelMass;
            this._mass = Basemass;
            this._frictionofweels = frictionofweels;
        }

        public void MoveCarOnCircle(float a, float b, float r, float timeElapsed)
        {
            //distanceToEndOfTheTrack = positionofcar - positionofBorder
            // r = radiusofTurn + distanceToEndOfTheTrack
            // Wyznaczamy nowy kąt, uwzględniając czas i prędkość


            float newAngle = Speed * timeElapsed / r;

            // Wyznaczamy nowe współrzędne X i Y samochodu
            float x = a + r * (float)Math.Cos(newAngle);
            float y = b + r * (float)Math.Sin(newAngle);

            //return new Tuple<float, float>(x, y);
        }
        // Run in the loop
        public void RunPhysic()
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeSinceLastExecution = currentTime - _lastExecutionTime;
            this._lastExecutionTime = currentTime;

            float AirR = AirResistance();
            float FF = FrictionForce(); // siła która przeciwdziała sile dośrodkowej
            float wheelFriction = 60;
            this._currentAcceleration = Acceleration() - AirR - wheelFriction;
            if (Acceleration() < AirR + wheelFriction) this._currentAcceleration = 0;
            this.Speed += _currentAcceleration / _mass; // * time
            this._fuelMass -= this._currentHorsePower * FuelBurningRatio; // * time
        }
        public void ConvertSpeedToVectors()
        {

        }
        //sila odsrodkowa
        public float CentrifugalForce(float speed, float radius, float mass)
        {
            return mass * (float)Math.Pow(speed, 2) * radius;
        }
        //siła tarcia
        public float FrictionForce()
        {
            return (float)(_mass * _frictionofweels * accelerationOfGravity);
            //To DO include trackAngle  * this.trackAngle
        }
        //Opór powietrza
        public float AirResistance()
        {
            return (float)(0.5 * airDensity * (float)Math.Pow(Speed, 2) * carAirDynamic * frontSurface);
        }
        // ile samochód się oddala/przybliża do środka
        private float Acceleration()
        {
            return this._currentHorsePower;
        }

        public float IscentrifugalForceEffectig(float speed, float radiusCar, float radiusOfWells, float mass, float frictionOfWeels)
        {
            float frictionDueToCentrifugalForce = CentrifugalForce(speed, radiusCar, mass) * (float)Math.Cos(trackAngle);
            float frictionAll = frictionDueToCentrifugalForce + FrictionForce();
            float centrifugalForce = CentrifugalForce(speed, radiusCar, mass);

            //nie ma pośligu
            if (frictionAll >= centrifugalForce)
            {
                return 0;
            }
            //jest poślizg
            else
            {
                return 1;
            }
        }

        public float Street(float speed)
        {
            return speed * 3;
        }
        public float MileageFuel(float speed, float mass)
        {
            return speed * mass;
        }
        public float Velocity()
        {
            return 0;
        }
    }
}