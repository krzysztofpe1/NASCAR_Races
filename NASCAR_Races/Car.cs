using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace NASCAR_Races
{
    public class Car : Physics
    {
        public bool IsDisposable { get; set; } = false;
        public bool Started { get; set; } = false;

        public string CarName { get; private set; }

        private Worldinformation _worldInfo;
        public Car(float x, float y, float weight, string carName, float maxHorsePower, Worldinformation worldInfo) : base(x, y, weight, 0.3f, maxHorsePower, worldInfo)
        {
            CarName = carName;
            _worldInfo = worldInfo;
        }
        Random random = new Random();
        public void Move()
        {
            _rightCircle = new Point(int.MaxValue, int.MaxValue);
            _leftCircle = new Point(0, 0);
            _neighbouringCars = new List<Car>();
            int counter = 0;
            while (!IsDisposable)
            {
                if (!Started) continue;
                //refreshing neighbouring cars list every 10 iterations
                if (++counter >= 10)
                //if (true)
                {
                    counter = 0;
                    var partOfCircuit = WhatPartOfCircuitIsCarOn();
                    _neighbouringCars = _worldInfo.NearbyCars(this);
                    int distanceToOpponentOnLeft = (int)DistanceToOpponentOnLeft();
                    switch (partOfCircuit)
                    {
                        case Worldinformation.CIRCUIT_PARTS.LEFT_TURN:

                            break;
                        case Worldinformation.CIRCUIT_PARTS.RIGHT_TURN:

                            break;
                        case Worldinformation.CIRCUIT_PARTS.TOP:
                            //Car will enter "left" turn
                            if (_neighbouringCars.Count > 0) FindSafeCircle((int)Y, false);
                            else FindCircle((int)Y, false);
                            break;
                        case Worldinformation.CIRCUIT_PARTS.BOTTOM:
                            //Car will enter "right" turn
                            if(_neighbouringCars.Count > 0)FindSafeCircle((int)Y, true);
                            else FindCircle((int)Y, true);
                            break;
                        case Worldinformation.CIRCUIT_PARTS.PIT:
                            State = STATE.PIT;
                            FindSafeCircle((int)Y, true);
                            break;
                    }
                }
                RunPhysic();
                Thread.Sleep(10);
            }
        }
    }
}
