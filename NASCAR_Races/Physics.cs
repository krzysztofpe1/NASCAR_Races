using System;
using System.Runtime.InteropServices;

namespace NASCAR_Races
{
    public class Physics
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Speed { get; private set; } = 0;
        public float HeadingAngle { get; set; } = 0;

        const float accelerationOfGravity = 9.81f;
        const float trackAngle = 10;
        const float airDensity = 1.225f;
        const float frontSurface = 2.5f;
        const float carAirDynamic = 0.35f;

        private float _currentAcceleration;
        private float _mass;
        private float _frictionofweels;
        private System.DateTime _lastExecutionTime;

        private Point _leftCircle;
        private Point _rightCircle;

        private float _turnRadius;

        protected float GasPedalDepression;

        protected float FuelMass;
        protected float FuelCapacity;
        protected float FuelBurningRatio = 0.5f;

        protected float MaxHorsePower;
        protected float CurrentHorsePower = 4000; // Force should be in the Future

        private double currentTurnAngle = -Math.PI / 2;

        private Worldinformation _worldInf;

        public Physics() { }
        public Physics(float x, float y, float mass, float fuelCapacity, float frictionofweels, Worldinformation worldInfo)
        {
            X= x;
            Y= y;
            _mass = mass;
            _leftCircle = worldInfo.LeftCircle;
            _rightCircle = worldInfo.RightCircle;
            _turnRadius = worldInfo.TurnRadius;
            FuelMass = fuelCapacity;
            FuelCapacity = fuelCapacity;
            _frictionofweels = frictionofweels;
            _lastExecutionTime = DateTime.Now;

            _worldInf = worldInfo;
        }
        // Run in the loop
        public void RunPhysic()
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeSinceLastExecution = currentTime - _lastExecutionTime;

            //float AirR = AirResistance();
            //float FF = FrictionForce(); // siła która przeciwdziała sile dośrodkowej
            //float wheelFriction = 60;
            //_currentAcceleration = Acceleration() - AirR - wheelFriction;
            //if (Acceleration() < AirR + wheelFriction) _currentAcceleration = 0;
            //Speed += _currentAcceleration / _mass; // * time
            Speed = 140f;
            //FuelMass -= CurrentHorsePower * FuelBurningRatio; // * time
            
            if (X > _rightCircle.X)
            {
                MoveCarOnCircle((float)timeSinceLastExecution.TotalSeconds, true, _rightCircle);
            }
            else if(X < _leftCircle.X)
            {
                MoveCarOnCircle((float)timeSinceLastExecution.TotalSeconds, true, _leftCircle);
            }
            //dodac else if dla lewego okregu
            else
            {
                if (Y <= _worldInf.CanvasHeight / 2)
                    X -= Speed * (float)timeSinceLastExecution.TotalSeconds;
                else
                    X += Speed * (float)timeSinceLastExecution.TotalSeconds;
            }

            _lastExecutionTime = currentTime;
        }

        private void MoveCarOnCircle(float timeElapsed, bool rightCircleControll,Point circle)
        {
            //distanceToEndOfTheTrack = positionofcar - positionofBorder
            float r = DistanceFromPointToPoint(X, Y, circle.X, circle.Y);
            // Wyznaczamy nowy kąt, uwzględniając czas i prędkość
            float a=0, b=0;
            if(rightCircleControll)
            {
                a= circle.X;
                b= circle.Y;
            }

            currentTurnAngle += Speed *timeElapsed/ r;
            // Wyznaczamy nowe współrzędne X i Y samochodu
            X = a + r * (float)Math.Cos(-currentTurnAngle);
            Y = b + r * (float)Math.Sin(-currentTurnAngle);

            //return new Tuple<float, float>(x, y);
        }
        
        public void ConvertSpeedToVectors()
        {

        }
        public float DistanceFromPointToPoint(float x1,float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

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
            return CurrentHorsePower;
        }

        public float IscentrifugalForceEffectig()
        {
            float frictionDueToCentrifugalForce = CentrifugalForce(Speed, HeadingAngle, _mass) * (float)Math.Cos(trackAngle);
            float frictionAll = frictionDueToCentrifugalForce + FrictionForce();
            float centrifugalForce = CentrifugalForce(Speed, HeadingAngle, _mass);

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
    }
}