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
        public List<Car> listOfCars { get; set; }
        public RaceManager(int straightLength, int turnRadius, int pitPosY, int turnCurveRadius, PictureBox mainPictureBox)
        {
            this.straightLength = straightLength;
            this.turnRadius = turnRadius;
            this.pitPosY = pitPosY;
            this.turnCurveRadius = turnCurveRadius;
        }

        public List<Car> CreateListOfCars(int numberOfCars)
        {
            listOfCars = new List<Car>();
            for (int i = 0; i < numberOfCars; i++)
            {
                Car car = new Car(570, 550, 1000, 0);
                listOfCars.Add(car);
            }
            return listOfCars;
        }

        public void MoveCars()
        {
            foreach (Car car in listOfCars)
            {
                
                car.Move();
            }
        }
    }
}
