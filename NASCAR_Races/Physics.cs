using System;
namespace NASCAR_Races
{
    internal class Physics
    {
        const float accelerationOfGravity = 9.81;
        const float trackAngle = 10;
        const float airDensity = 1.225;
        const float frontSurface = 2.5;
        const float carAirDynamic = 0.35;



        private float currentAcceleration;
        private float mass;
        private float currentHorsePower;
        private float radiusofCar;
        private float radiusofWells;
        private float frictionofweels;
        private float radiusCar = 0;
        private float fuelMass;

        protected float fuelBurningRatio = 0.5;
        protected float acceleration = 0;
        protected float speed = 0;
        protected System.DateTime timeFromStart = 0;
        protected DateTime timecurrnet = 0;
        public Physics(float Basemass, float frictionofweels, float fuelMass)
        {
            timeFromStart = new System.DateTime.Now();
            this.fuelMass = fuelMass;
            this.mass = Basemass;
            this.frictionofweels = frictionofweels;
        }
        public RunPhysic()
        {
            //timecurrnet 
            this.acceleration = acceleration() - airResistance() - frictionForce();
            this.speed += acceleration;
            this.fuelMass -= this.currentHorsePower * fuelBurningRatio;
        }
        //sila odsrodkowa
        public static float CentrifugalForce(float speed, float radius, float mass)
        {
            return mass * pow(speed, 2) * radius;
        }
        //siła tarcia
        public static float FrictionForce(float mass, float frictionofweels)
        {
            return this.mass * this.frictionofweels * this.accelerationOfGravity;
            //To DO include trackAngle  * this.trackAngle
        }
        //Opór powietrza
        public static float AirResistance(float speed)
        {
            return 0.5 * this.airDensity * pow(this.speed, 2) * this.carAirDynamic * this.frontSurface;
        }
        // ile samochód się oddala/przybliża do środka
        private static float Acceleration()
        {
            return this.currentHorsePower / this.mass;
        }

        public static float IscentrifugalForceEffectig(float speed, float radiusCar, float radiusOfWells, float mass, float frictionOfWeels)
        {
            float frictionDueToCentrifugalForce = centrifugalForce(speed, radius, mass) * Math.Cos(Double(trackAngle));
            float frictionAll = frictionDueToCentrifugalForce + frictionForce(mass, frictionOfWeels);
            float centrifugalForce = centrifugalForce(speed, radius, mass);

            //nie ma pośligu
            if(frictionAll >= centrifugalForce)
            {
                return 0;
            }
            //jest poślizg
            else
            {
                return 1;
            }
        }

        public static float Street(float speed)
        {
            return speed * 3;
        }
        public static float MileageFuel(float speed, float mass)
        {
            return speed * mass;
        }
        public static float Velocity()
        {
            return 0;
        }
    }
}