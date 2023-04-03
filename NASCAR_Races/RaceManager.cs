using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class RaceManager
    {
        const int X_MAX = 900;
        const int X_MIN = 280;
        const int X_MID = (X_MAX + X_MIN) / 2;
        const int Y_MAX = 580;
        const int Y_MIN = 70;
        const int Y_MID = (Y_MAX + Y_MIN) / 2;

        int straightLength;
        int turnRadius;
        int pitPosY;
        int turnCurveRadius;
        public RaceManager(int straightLength, int turnRadius, int pitPosY, int turnCurveRadius)
        {
            this.straightLength = straightLength;
            this.turnRadius = turnRadius;
            this.pitPosY = pitPosY;
            this.turnCurveRadius = turnCurveRadius;
        }

        public static List<Car> CreateListOfCars(int numberOfCars)
        {
            List<Car> litsOfCars = new List<Car>();

            for (int i = 0; i < numberOfCars; i++)
            {
                Engine engine = new Engine(1000, 1000, 200, 1, 20);
                Car car = new Car(570, 550, 1000, 0, engine);
                litsOfCars.Add(car);
            }
            return litsOfCars;
        }

        internal static void MoveCars(List<Car> listOfCars)
        {
            foreach (Car car in listOfCars)
            {
                if (car.engine == null)
                {
                    throw new InvalidOperationException("Car does not have an engine");
                }

                // Implementation of cars moving

                // Bot
                if (car.x < X_MAX && car.x > X_MIN && car.y > Y_MID) car.x += Physics.Street(car.engine.speed);

                // Top
                if (car.x < X_MAX && car.x > X_MIN && car.y < Y_MID) car.x -= Physics.Street(car.engine.speed);

                // Right
                if (car.x >= X_MAX)
                {
                    if(car.y > Y_MID) car.x += Physics.Street(car.engine.speed);
                    else car.x -= Physics.Street(car.engine.speed);

                    car.y -= Physics.Street(car.engine.speed);
                }

                // Left
                if (car.x <= X_MIN)
                {
                    if (car.y > Y_MID) car.x += Physics.Street(car.engine.speed);
                    else car.x -= Physics.Street(car.engine.speed);

                    car.y += Physics.Street(car.engine.speed);
                }
            }
        }
    }
}
