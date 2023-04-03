namespace NASCAR_Races
{
    public class Engine
    {

        public float fuelCapacity;
        public float fuel { get; set; }
        public float maxSpeed;
        public float speed;

        public Engine()
        {

        }

        public Engine(float fuelCapacity, float fuel, float maxSpeed, float speed)
        {
            this.fuelCapacity = fuelCapacity;
            this.fuel = fuel;
            this.maxSpeed = maxSpeed;
            this.speed = speed;
        }
    }
}