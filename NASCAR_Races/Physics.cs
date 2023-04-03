namespace NASCAR_Races
{
    internal class Physics
    {

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