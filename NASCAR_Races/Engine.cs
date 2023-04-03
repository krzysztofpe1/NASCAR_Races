namespace NASCAR_Races
{
    public class Engine
    {

        public float fuelCapacity;
        public float fuel { get; set; }
        public float maxSpeed;
        public float speed;
        public float acceleration;

        public Engine()
        {

        }

        public Engine(float fuelCapacity, float fuel, float maxSpeed, float speed, float acceleration)
        {
            this.fuelCapacity = fuelCapacity;
            this.fuel = fuel;
            this.maxSpeed = maxSpeed;
            this.speed = speed;
            this.acceleration = acceleration;
        }
    }
}