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
        public enum STATE
        {
            ON_CIRCUIT,
            OFF_CIRCUIT,
            ON_WAY_TO_PIT_STOP,
            PIT_STOP
        }

        public STATE State { get; private set; } = Car.STATE.ON_CIRCUIT;

        public bool IsDisposable { get; set; } = false;
        public bool Started { get; set; } = false;


        public string CarName { get; private set; }



        private Worldinformation _worldInfo;
        public Car(float x, float y, float weight, float fuelCapacity, string carName, float maxHorsePower, Worldinformation worldInfo) : base(x, y, weight, fuelCapacity, 0.3f, maxHorsePower, worldInfo)
        {
            CarName = carName;
            _worldInfo = worldInfo;
        }
        Random random = new Random();
        public void Move()
        {
            _rightCircle=new Point(int.MaxValue, int.MaxValue);
            _leftCircle=new Point(0,0);
            _neighbouringCars = new List<Car>();
            int counter = 0;
            while (!IsDisposable)
            {
                if (!Started) continue;
                //refreshing neighbouring cars list every 10 iterations
                var partOfCircuit = WhatPartOfCircuitIsCarOn();
                if (partOfCircuit == Worldinformation.CIRCUIT_PARTS.BOTTOM || partOfCircuit == Worldinformation.CIRCUIT_PARTS.TOP)
                    if (++counter >= 10)
                    //if (true)
                    {
                        counter = 0;
                        _neighbouringCars = _worldInfo.NearbyCars(this);
                        int distanceToOpponentOnLeft = (int)DistanceToOpponentOnLeft();
                        switch (_worldInfo.WhatPartOfCircuitIsCarOn(this))
                        {
                            case Worldinformation.CIRCUIT_PARTS.LEFT_TURN:

                                break;
                            case Worldinformation.CIRCUIT_PARTS.RIGHT_TURN:

                                break;
                            case Worldinformation.CIRCUIT_PARTS.TOP:
                                //Car will enter "left" turn
                                FindSafeCircle((int)Y, false);
                                //FindCircle((int)Y, false);
                                break;
                            case Worldinformation.CIRCUIT_PARTS.BOTTOM:
                                //Car will enter "right" turn
                                FindSafeCircle((int)Y, true);
                                //FindCircle((int)Y, true);
                                break;
                            case Worldinformation.CIRCUIT_PARTS.PIT:

                                break;
                        }
                    }
                RunPhysic();
                Thread.Sleep(10);
            }
        }
    }
}
