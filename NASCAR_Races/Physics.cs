namespace NASCAR_Races
{
    internal class Physics
    {
        const float accelerationOfGravity = 9.81;
        const float trackAngle = 9.81;

        //sila odsrodkowa
        internal static float centrifugalForce(float speed, float radius, float mass)
        {
            return mass * pow(speed, 2) * radius;
        }
        //siła tarcia
        internal static float frictionForce(float mass, float frictionofweels)
        {
            return mass * frictionofweels * accelerationOfGravity * trackAngle;
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