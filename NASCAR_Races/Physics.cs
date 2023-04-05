namespace NASCAR_Races
{
    internal class Physics
    {
        const float accelerationOfGravity = 9.81;
        const float trackAngle = 10;
        const float airDensity = 1.225;
        const float frontSurface = 2.5;
        const float carAirDynamic = 0.35;

        //sila odsrodkowa
        internal static float centrifugalForce(float speed, float radius, float mass)
        {
            return mass * pow(speed, 2) * radius;
        }
        //siła tarcia
        internal static float frictionForce(float mass, float frictionofweels)
        {
            return mass * frictionofweels * accelerationOfGravity * trackAngle;
            //To DO include trackAngle 
        }
        //Opór powietrza
        internal static float airResistance(float speed)
        {
            return 0.5 * airDensity * pow(speed, 2) * carAirDynamic * frontSurface;
        }
        // ile samochód się oddala/przybliża do środka
        internal static float iscentrifugalForceEffectig(float speed, float radius, float mass, float frictionofweels)
        {
            return (frictionForce(mass, frictionofweels) - centrifugalForce(speed, radius, mass)) / mass;
        }
        internal static float Street(float speed)
        {
            return speed * 3;
        }

        internal static float MileageFuel(float speed, float weight)
        {
            return speed * weight;
        }

        internal static float Velocity()
        {
            return 0;
        }
    }
}