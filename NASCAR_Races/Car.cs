using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Car(float x, float y, float weight, float fuelCapacity, string carName, Worldinformation worldInfo) : base(x, y, weight, fuelCapacity, 0.3f, worldInfo)
        {
            CarName = carName;
            _worldInfo = worldInfo;
        }
        Random random = new Random();
        public void Move()
        {
            _neighbouringCars = _worldInfo.NearbyCars(this);
            int counter = 0;
            while (!IsDisposable)
            {
                if (!Started) continue;
                //refreshing neighbouring cars list every 10 iterations
                if (++counter >= 10)
                {
                    counter = 0;
                    _neighbouringCars = _worldInfo.NearbyCars(this);
                    if (_neighbouringCars.Count != 0)
                    //if(true)
                    {
                        //TODO
                        //obliczanie kola po jakim auto musi przejechac, zeby nie uderzyc w inne auto
                        List<Double> points = new List<Double>();
                        switch (_worldInfo.WhatPartOfCircuitIsCarOn(this))
                        {
                            case Worldinformation.CIRCUIT_PARTS.LEFT_TURN:

                                break;
                            case Worldinformation.CIRCUIT_PARTS.RIGHT_TURN:

                                break;
                            case Worldinformation.CIRCUIT_PARTS.TOP:
                                //Car will enter "left" turn
                                //TODO
                                //jezeli auto bedzie w tej czesci toru, musi obliczyc, czy starczy mu paliwa i opon na jeszcze jedno okrazenie
                                //tym samym, czy musi zjechac do pitu
                                points = FindCircle(_worldInfo.x1, (int)Y,
                                                    _worldInfo.x1, (int)(_worldInfo.CanvasCenterY + (_worldInfo.CanvasCenterY - Y)),
                                                    _worldInfo.x1-_worldInfo.TurnRadius+(int)DistanceToOpponentOnLeft(), _worldInfo.CanvasCenterY);
                                _leftCircle = new Point((int)points[0], (int)points[1]);
                                _circleRadius = (int)points[2];
                                break;
                            case Worldinformation.CIRCUIT_PARTS.BOTTOM:
                                //Car will enter "right" turn
                                points = FindCircle(_worldInfo.x2, (int)Y,
                                                    _worldInfo.x2, (int)(_worldInfo.CanvasCenterY - (Y - _worldInfo.CanvasCenterY)),
                                                    _worldInfo.x2+_worldInfo.TurnRadius-(int)DistanceToOpponentOnLeft(), _worldInfo.CanvasCenterY);
                                _rightCircle = new Point((int)points[0], (int)points[1]);
                                _circleRadius = (int)points[2];
                                break;
                            case Worldinformation.CIRCUIT_PARTS.PIT:

                                break;
                        }
                    }
                    else
                    {
                        Worldinformation.CIRCUIT_PARTS partOfCircuit = _worldInfo.WhatPartOfCircuitIsCarOn(this);
                        if (partOfCircuit != Worldinformation.CIRCUIT_PARTS.LEFT_TURN && partOfCircuit != Worldinformation.CIRCUIT_PARTS.RIGHT_TURN)
                        {
                            _leftCircle = _leftPerfectCircle;
                            _rightCircle = _rightPerfectCircle;
                            _circleRadius = _perfectCircleRadius;
                        }
                    }
                }
                RunPhysic();
                Thread.Sleep(10);
            }
        }
        //works only on straights
        public Point FrontLeftCorner()
        {
            //TODO
            //wprowadzic jakis mechanizm ktory dokona kalkulacji gdzie dokladnie znajduje sie rog; wykorzystac HeadingAngle podany w STOPPNIACH NIE RADIANACH
            if (_worldInfo.WhatPartOfCircuitIsCarOn(this) == Worldinformation.CIRCUIT_PARTS.TOP) return new Point((int)(X - Length / 2), (int)(Y + Width / 2));
            if (_worldInfo.WhatPartOfCircuitIsCarOn(this) == Worldinformation.CIRCUIT_PARTS.BOTTOM) return new Point((int)(X + Length / 2), (int)(Y - Width / 2));
            return new Point();
        }
        //works only on straights
        public Point ReadRightCorner()
        {
            if (_worldInfo.WhatPartOfCircuitIsCarOn(this) == Worldinformation.CIRCUIT_PARTS.TOP) return new Point((int)(X + Length / 2), (int)(Y - Width / 2));
            if (_worldInfo.WhatPartOfCircuitIsCarOn(this) == Worldinformation.CIRCUIT_PARTS.BOTTOM) return new Point((int)(X - Length / 2), (int)(Y + Width / 2));
            return new Point();
        }
        //return true if there is at least one opponent on the left side
        private bool AreThereOpponentsOnSide(Worldinformation.CIRCUIT_PARTS partOfCircuit)
        {
            foreach (Car car in _neighbouringCars)
            {
                switch (partOfCircuit)
                {
                    case Worldinformation.CIRCUIT_PARTS.LEFT_TURN:
                        //if (car.X < X) return true;
                        break;
                    case Worldinformation.CIRCUIT_PARTS.RIGHT_TURN:
                        //if(car.X > X) return true;
                        break;
                    case Worldinformation.CIRCUIT_PARTS.TOP:
                        //Car will enter "left" turn
                        if (car.Y > Y) return true;
                        break;
                    case Worldinformation.CIRCUIT_PARTS.BOTTOM:
                        //Car will enter "right" turn
                        if (car.Y < Y) return true;
                        break;
                    case Worldinformation.CIRCUIT_PARTS.PIT:
                        return false;
                }
            }
            return false;
        }


    }
}
