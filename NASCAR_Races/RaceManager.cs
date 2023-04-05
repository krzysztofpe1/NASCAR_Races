using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races
{
    public class RaceManager
    {
        private int _straightLength;
        private int _turnRadius;
        private int _pitPosY;
        private int turnCurveRadius;
        public List<CarThread> ListOfCarThreads;
        public List<Car> ListOfCars;
        public RaceManager(int straightLength, int turnRadius, int pitPosY, int turnCurveRadius, PictureBox mainPictureBox)
        {
            _straightLength = straightLength;
            _turnRadius = turnRadius;
            _pitPosY = pitPosY;
            this.turnCurveRadius = turnCurveRadius;
        }

        public List<Car> CreateListOfCars(int numberOfCars)
        {
            ListOfCarThreads = new List<CarThread>();
            ListOfCars = new List<Car>();
            for (int i = 0; i < numberOfCars; i++)
            {
                CarThread car = new(570, 550, 1000, 70);
                ListOfCarThreads.Add(car);
                ListOfCars.Add((Car)car);
            }
            return ListOfCars;
        }

        public void StartRace()
        {
            ListOfCarThreads.ForEach(carThread => { carThread.StartCar(); });
        }
    }
}
