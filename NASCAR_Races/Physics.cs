using System;
namespace NASCAR_Races
{
    public class Physics
    {
        const float accelerationOfGravity = 9.81f;
        const float trackAngle = 10;
        const float airDensity = 1.225f;
        const float frontSurface = 2.5f;
        const float carAirDynamic = 0.35f;


        private float currentAcceleration;
        private float mass;
        private float currentHorsePower = 4000; // Force should be in the Future
        private float radiusofCar;
        private float radiusofWells;
        private float frictionofweels;
        private float radiusCar = 0;
        private float fuelMass;

        protected float fuelBurningRatio = 0.5f;
        protected float acceleration = 0;
        protected float speed = 0;
        protected System.DateTime timeFromStart;
        protected DateTime timecurrnet;

        public Physics() { }
        public Physics(float Basemass, float frictionofweels, float fuelMass)
        {
            timeFromStart = DateTime.Now;
            this.fuelMass = fuelMass;
            this.mass = Basemass;
            this.frictionofweels = frictionofweels;
        }

        public static Tuple<double, double> MoveCarOnCircle(double a, double b, double r, double speed, double timeElapsed)
        {
            // Wyznaczamy nowy kąt, uwzględniając czas i prędkość
            double newAngle = speed * timeElapsed / r;

            // Wyznaczamy nowe współrzędne X i Y samochodu
            double x = a + r * Math.Cos(newAngle);
            double y = b + r * Math.Sin(newAngle);

            return new Tuple<double, double>(x, y);
        }
        // Run in the loop
        public void RunPhysic()
        {
            //timecurrnet 
            float AirR = AirResistance();
            float FF = FrictionForce();
            float wheelFriction = 60;
            this.acceleration = Acceleration() - AirR - wheelFriction;
            if (Acceleration() < AirR + wheelFriction) this.acceleration = 0;
            this.speed += acceleration/mass; // * time
            this.fuelMass -= this.currentHorsePower * fuelBurningRatio; // * time
        }
        //sila odsrodkowa
        public float CentrifugalForce(float speed, float radius, float mass)
        {
            return mass * (float)Math.Pow(speed, 2) * radius;
        }
        //siła tarcia
        public float FrictionForce()
        {
            return (float)(mass * frictionofweels * accelerationOfGravity);
            //To DO include trackAngle  * this.trackAngle
        }
        //Opór powietrza
        public float AirResistance()
        {
            return (float)(0.5 * airDensity * (float)Math.Pow(speed, 2) * carAirDynamic * frontSurface);
        }
        // ile samochód się oddala/przybliża do środka
        private float Acceleration()
        {
            return this.currentHorsePower;
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