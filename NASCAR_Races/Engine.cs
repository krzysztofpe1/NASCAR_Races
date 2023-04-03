namespace NASCAR_Races
{
    public class Engine
    {

        float fuelCapacity;
        float fuel { get; set; }
        float maxSpeed;
        float speed;

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