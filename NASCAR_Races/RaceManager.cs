using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class RaceManager
    {
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
                Engine engine = new Engine(1000, 1000, 200, 1);
                Car car = new Car(570, 100, 1000, 0, engine);
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

                car.x += Physics.Street(car.engine.speed);
            }
        }
    }
}
