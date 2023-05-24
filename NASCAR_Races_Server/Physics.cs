using System;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.VisualBasic.Logging;

namespace NASCAR_Races
{
    public class Physics
    {
        public float X { get; private set; } = 0;
        public float Y { get; private set; } = 0;
        public float Length { get; private set; }
        public float Width { get; private set; }
        private float _speed;
        public float Speed
        {
            get { return _speed; }
            private set
            {
                if (value < 0) _speed = 0;
                else _speed = value;
            }
        }
        public float HeadingAngle { get; set; } = 0;

        const float accelerationOfGravity = 9.81f;
        const float trackAngle = 0.175f;
        const float airDensity = 1.225f;
        const float frontSurface = 2.5f;
        const float carAirDynamic = 0.35f;
        //change to private for logs only
        public float _currentAcceleration;
        private float _mass;
        private float _frictionofweels;
        private bool _turnToPit = false;
        private System.DateTime _lastExecutionTime;
        private float _previosAtanPit = 0;
        private float _currentAtanPit = 0;

        protected Point _leftCircle { get; set; }
        protected Point _rightCircle { get; set; }
        protected int _circleRadius { get; set; }
        protected Point _pitPos { get; set; }

        private float _UseOftires = 0.5f;

        public float FuelMass { get; private set; }
        protected float FuelBurningRatio = 0.0001f;

        public float MaxHorsePower { get; set; }
        public float CurrentHorsePower { get; set; }
        protected float BrakesForce = 50000;

        protected List<Car> _neighbouringCars;

        protected bool _recalculateHeadingAngle { get; set; } = true;
        protected bool _recalculatePit { get; set; } = true;

        private double currentTurnAngle = -Math.PI / 2;

        private Worldinformation _worldInf;
        private int _carSafeDistance;

        public enum STATE
        {
            ON_CIRCUIT,
            OFF_CIRCUIT,
            ON_WAY_TO_PIT_STOP,
            PIT,
            PIT_STOPPED
        }

        public STATE State { get; protected set; } = STATE.ON_CIRCUIT;

        public Physics(float x, float y, float mass, float frictionofweels, float maxHorsePower, Worldinformation worldInfo)
        {
            X = x;
            Y = y;
            Length = worldInfo.CarLength;
            Width = worldInfo.CarWidth;
            _mass = mass;
            _frictionofweels = frictionofweels;
            MaxHorsePower = maxHorsePower;
            CurrentHorsePower = MaxHorsePower;
            _lastExecutionTime = DateTime.Now;

            _worldInf = worldInfo;
            _carSafeDistance = _worldInf.CarsSafeDistance;
            FuelMass = _worldInf.CarInitialFuelMass;
        }
        // Run in the loop
        public void RunPhysic()
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeSinceLastExecution = currentTime - _lastExecutionTime;
            _lastExecutionTime = currentTime;
            //timeSinceLastExecution *= 1.5;
            //float FF = FrictionForce(); // siła która przeciwdziała sile dośrodkowej
            float wheelFriction = 60;
            float airR = AirResistance();
            _currentAcceleration = (AccelerationForce() - airR - wheelFriction) / _mass;
            Speed += (float)(_currentAcceleration * (float)timeSinceLastExecution.TotalSeconds);

            //if (Acceleration() < AirR + wheelFriction) _currentAcceleration = 0;
            //FuelMass -= CurrentHorsePower * FuelBurningRatio; // * time
            var partOfCircuit = WhatPartOfCircuitIsCarOn();
            if (partOfCircuit == Worldinformation.CIRCUIT_PARTS.RIGHT_TURN)
            {
                MoveCarOnCircle((float)timeSinceLastExecution.TotalSeconds, true, _rightCircle);
            }
            else if (partOfCircuit == Worldinformation.CIRCUIT_PARTS.LEFT_TURN)
            {
                MoveCarOnCircle((float)timeSinceLastExecution.TotalSeconds, false, _leftCircle);
            }
            else
            {
                MoveCarOnStraight((float)timeSinceLastExecution.TotalSeconds, partOfCircuit);
                _recalculateHeadingAngle = true;
            }
            //Managing Fuel
            FuelMass -= (float)(CurrentHorsePower * FuelBurningRatio * timeSinceLastExecution.TotalSeconds);
            if (FuelMass < _worldInf.CarInitialFuelMass / 4) State = STATE.ON_WAY_TO_PIT_STOP;
            else State = STATE.ON_CIRCUIT;
        }
        private void MoveCarOnCircle(float timeElapsed, bool rightCircleControll, Point circle)
        {
            if (_recalculateHeadingAngle)
            {
                currentTurnAngle = CalculateEnteringAngle(rightCircleControll);
                _recalculateHeadingAngle = false;
            }
            float r = _circleRadius;

            float a = circle.X;
            float b = circle.Y;

            currentTurnAngle += Speed * timeElapsed / r;
            HeadingAngle = -(float)((currentTurnAngle + Math.PI / 2) * (180.0 / Math.PI));

            X = a + r * (float)Math.Cos(-currentTurnAngle);
            Y = b + r * (float)Math.Sin(-currentTurnAngle);
        }
        private void MoveCarOnStraight(float timeElapsed, Worldinformation.CIRCUIT_PARTS partOfCircuit)
        {
            CurrentHorsePower = MaxHorsePower;
            if (partOfCircuit == Worldinformation.CIRCUIT_PARTS.TOP)
            {
                //TOP
                if (State == STATE.ON_WAY_TO_PIT_STOP)
                {
                    if (DistanceToOpponentOnLeft() > _carSafeDistance)
                    {
                        Y += 0.25f;
                        CurrentHorsePower = MaxHorsePower;
                    }
                    else if (_worldInf.DistanceToEdgeOfTrack(this, false) > 1)
                    {
                        CurrentHorsePower = 0;
                        (float, float, float) opponentInFront = DistanceToSpeedAndHPOfOpponentInFront();
                        if (opponentInFront.Item1 < _worldInf.CarsSafeDistance || opponentInFront.Item2 < Speed) Speed -= 0.25f;
                    }
                    else CurrentHorsePower = MaxHorsePower;
                    X -= Speed * timeElapsed;
                    HeadingAngle = 0;
                    return;
                }
                // if car in front is slower then go to the left
                (float, float, float) temp = DistanceToSpeedAndHPOfOpponentInFront();
                if (temp.Item1 < float.MaxValue && temp.Item3 < MaxHorsePower)
                {
                    CurrentHorsePower = temp.Item3;
                    if (temp.Item2 < Speed) Speed -= 0.25f;
                    if (DistanceToOpponentOnLeft() > _carSafeDistance)
                    {
                        Y += 0.25f;
                    }
                }
                else if ((DistanceToOpponentOnRight() > _carSafeDistance || DistanceToOpponentOnRight() == _worldInf.DistanceToEdgeOfTrack(this)) && _worldInf.DistanceToEdgeOfTrack(this) > 1)
                {
                    CurrentHorsePower = MaxHorsePower;
                    //notBraking();
                    Y -= 0.25f;
                }
                else Debug.WriteLine("Out of context");
                X -= Speed * timeElapsed;
                HeadingAngle = 0;

            }
            else if (partOfCircuit == Worldinformation.CIRCUIT_PARTS.BOTTOM)
            {
                //BOTTOM
                (float, float, float) temp = DistanceToSpeedAndHPOfOpponentInFront();
                if (temp.Item1 < float.MaxValue && temp.Item3 < MaxHorsePower)
                {
                    CurrentHorsePower = temp.Item3;
                    if (temp.Item2 < Speed) Speed -= 0.25f;
                    if (DistanceToOpponentOnLeft() > _carSafeDistance)
                    {
                        Y -= 0.25f;
                    }
                }
                else if ((DistanceToOpponentOnRight() > _carSafeDistance || DistanceToOpponentOnRight() == _worldInf.DistanceToEdgeOfTrack(this)) && _worldInf.DistanceToEdgeOfTrack(this) > 1)
                {
                    CurrentHorsePower = MaxHorsePower;
                    //notBraking();
                    Y += 0.25f;
                }
                X += Speed * timeElapsed;
                HeadingAngle = 180;
            }
            else if (partOfCircuit == Worldinformation.CIRCUIT_PARTS.PIT)
            {
                int bottomBorderPit = _worldInf.PitPosY + _worldInf.PenCircuitSize / 4 - (int)Length / 2;
                //int bottomBorderPit = 505;
                Debug.WriteLine(bottomBorderPit + " " + _worldInf.PitPosY);
                if (Y < bottomBorderPit && _pitPos.X - 100 > X)
                {
                    Y++;
                }
                if (Speed > _worldInf.CarMaxSpeedInPit)
                {
                    CurrentHorsePower = 0;
                    Speed -= 1f;
                }
                else
                {
                    CurrentHorsePower = MaxHorsePower;
                    Speed += 0.25f;
                }

                X += Speed * timeElapsed;
                HeadingAngle = 180;
                //float prevatn = 0;

                // do góry przed pitem
                int manover_size = (int)Length * 2;
                if (X > _pitPos.X - manover_size && X < _pitPos.X && (int)Y > (int)_pitPos.Y)
                {
                    _currentAtanPit = (float)(Math.Atan((-_pitPos.X + (X + manover_size / 2)) / 20.0) + Math.PI / 2) * 6;
                    Y = Y - (_currentAtanPit - _previosAtanPit);
                    _previosAtanPit = _currentAtanPit;
                }
                // w dol po picie
                else if (X > _pitPos.X && (int)Y < bottomBorderPit)
                {
                    _currentAtanPit = (float)(Math.Atan((X - (_pitPos.X + manover_size / 2)) / 10.0) + Math.PI / 2) * 6;
                    Y = Y + (_currentAtanPit - _previosAtanPit);
                    _previosAtanPit = _currentAtanPit;
                }
                else { _previosAtanPit = 0; }
                if (_pitPos.Y > Y)
                {
                    Debug.WriteLine("Error: ");
                }
                if (X >= _pitPos.X - 1 && X <= _pitPos.X + 1 && FuelMass + 5 < _worldInf.CarInitialFuelMass)
                {
                    State = STATE.PIT_STOPPED;
                    Thread.Sleep(1000);
                    FuelMass = _worldInf.CarInitialFuelMass;
                    _previosAtanPit = 0;
                    _lastExecutionTime = DateTime.Now;
                }

            }

            /*if (IscentrifugalForce(_circleRadius) != 0 && ((_leftPerfectCircle.Y > Y && X < _leftPerfectCircle.X + _circleRadius / 2) || (_rightPerfectCircle.Y < Y && X > _rightPerfectCircle.X - _circleRadius / 2)))
            {
                Braking(timeElapsed);
            }
            else
            {
                notBraking();
            }*/
        }
        public float DistanceFromPointToPoint(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }
        //sila odsrodkowa
        public float CentrifugalForce(float radius)
        {
            float CenForce = _mass * (float)Math.Pow(Speed, 2) / radius;
            //Console.WriteLine("Cen Force: " + CenForce);
            return CenForce;
        }
        //siła tarcia
        public float FrictionForce()
        {
            return (float)(_mass * _frictionofweels * accelerationOfGravity);
            //To DO include trackAngle  * this.trackAngle
        }
        //Opór powietrza
        public float AirResistance()
        {
            return (float)(0.5 * airDensity * (float)Math.Pow(Speed, 2) * carAirDynamic * frontSurface);
        }
        // ile samochód się oddala/przybliża do środka
        private float AccelerationForce()
        {
            float efficency = 1 - (Speed / 100);
            return CurrentHorsePower * efficency;
        }
        public float IscentrifugalForce(float radius)
        {
            //* (float)Math.Cos(trackAngle)
            float CF = CentrifugalForce(radius);
            float Fofx = CF * (float)Math.Cos(trackAngle);
            float Fofy = CF * (float)Math.Sin(trackAngle);

            float GravityForce = _mass * accelerationOfGravity;
            float GravityForce_X = GravityForce * (float)Math.Sin(trackAngle);
            float GravityForce_Y = GravityForce * (float)Math.Cos(trackAngle);

            float frictionAll = (GravityForce_Y + Fofy) * _frictionofweels;
            float XFroce = Fofx - GravityForce_X;

            //nie ma pośligu
            if (frictionAll >= Math.Abs(XFroce))
            {
                return 0;
            }
            //jest poślizg
            else
            {
                if (XFroce > 0)
                    return XFroce - frictionAll;
                else
                    return frictionAll - XFroce;
            }
        }
        protected void FindCircle(int y, bool righCircleControl)
        {
            if (State == STATE.ON_WAY_TO_PIT_STOP && !righCircleControl)
            {
                FindSafeCircle(y, righCircleControl);
                return;
            }

            _circleRadius = Math.Abs(y - _worldInf.CanvasCenterY);
            int x;
            float distanceToEdgeOfTrackInTheMiddleOfTurn = _worldInf.DistanceToEdgeOfTrack(this, false);
            if (DistanceToOpponentOnLeft() + 1 < distanceToEdgeOfTrackInTheMiddleOfTurn)
            {
                distanceToEdgeOfTrackInTheMiddleOfTurn -= Width;
            }
            if (righCircleControl)
            {
                x = _worldInf.x2 + _worldInf.TurnRadius - _circleRadius + _worldInf.PenCircuitSize / 2 - (int)distanceToEdgeOfTrackInTheMiddleOfTurn;
                _rightCircle = new Point(x, _worldInf.CanvasCenterY);
            }
            else
            {
                x = _worldInf.x1 - _worldInf.TurnRadius + _circleRadius - _worldInf.PenCircuitSize / 2 + (int)distanceToEdgeOfTrackInTheMiddleOfTurn;
                _leftCircle = new Point(x, _worldInf.CanvasCenterY);
            }
        }
        protected void FindSafeCircle(int y, bool righCircleControl)
        {
            if (State == STATE.ON_WAY_TO_PIT_STOP && !righCircleControl)
            {
                _circleRadius = Math.Abs(y - _worldInf.PitPosY) / 2;
                _leftCircle = new Point(_worldInf.x1 - ((_worldInf.DistanceToEdgeOfTrack(this, false) < _worldInf.CarPitStopEntryOffset) ? _worldInf.CarPitStopEntryOffset : 0), y + _circleRadius);
                return;
            }
            else if (State == STATE.PIT)
            {
                _circleRadius = 10 + (_worldInf.PitPosY - (_worldInf.CanvasCenterY - _worldInf.TurnRadius + _worldInf.PenCircuitSize / 2)) / 2;
                _rightCircle = new Point(_worldInf.x2 + _worldInf.CarPitStopEntryOffset, y - _circleRadius);
                return;
            }
            _circleRadius = Math.Abs(y - _worldInf.CanvasCenterY);
            if (righCircleControl)
            {
                _rightCircle = new Point(_worldInf.x2, _worldInf.CanvasCenterY);
            }
            else
            {
                _leftCircle = new Point(_worldInf.x1, _worldInf.CanvasCenterY);
            }
        }
        //return:
        //distance to opponent on right if there is any
        //distance to the edge of circuit
        protected float DistanceToOpponentOnRight()
        {
            float distance = _worldInf.DistanceToEdgeOfTrack(this);
            foreach (Car car in _neighbouringCars)
            {
                if (Math.Abs(X - car.X) > Length + car.Length / 2) continue;
                float temp;
                switch (_worldInf.WhatPartOfCircuitIsCarOn(this))
                {
                    case Worldinformation.CIRCUIT_PARTS.LEFT_TURN:

                        break;
                    case Worldinformation.CIRCUIT_PARTS.RIGHT_TURN:

                        break;
                    case Worldinformation.CIRCUIT_PARTS.TOP:
                        //Car will enter "left" turn
                        if (car.Y < Y)
                        {
                            //opponent is on the right side of this car
                            temp = (Y - Width / 2) - (car.Y + car.Width / 2);
                            if (temp < distance) distance = temp;
                        }
                        break;
                    case Worldinformation.CIRCUIT_PARTS.BOTTOM:
                        //Car will enter "right" turn
                        if (car.Y > Y)
                        {
                            //opponent is on the right side of this car
                            temp = (car.Y - car.Width / 2) - (Y + Width / 2);
                            if (temp < distance) distance = temp;
                        }
                        break;
                    case Worldinformation.CIRCUIT_PARTS.PIT:

                        break;
                }
            }
            return distance;
        }
        //returns:
        //distance to first opponent on the left, regardless of his X coordinates
        //distance to edge of track if there are no opponents on left side
        protected float DistanceToOpponentOnLeft()
        {
            float distance = _worldInf.DistanceToEdgeOfTrack(this, false);
            foreach (Car car in _neighbouringCars)
            {
                if (Math.Abs(X - car.X) > Length + car.Length / 2) continue;
                float temp;
                switch (_worldInf.WhatPartOfCircuitIsCarOn(this))
                {
                    case Worldinformation.CIRCUIT_PARTS.LEFT_TURN:

                        break;
                    case Worldinformation.CIRCUIT_PARTS.RIGHT_TURN:

                        break;
                    case Worldinformation.CIRCUIT_PARTS.TOP:
                        //Car will enter "left" turn
                        if (car.Y > Y)
                        {
                            //opponent is on the left side of this car
                            temp = car.Y - car.Width / 2 - (Y + Width / 2);
                            if (temp < distance) distance = temp;
                        }
                        break;
                    case Worldinformation.CIRCUIT_PARTS.BOTTOM:
                        //Car will enter "right" turn
                        if (car.Y < Y)
                        {
                            //opponent is on the left side of this car
                            temp = Y - Width / 2 - (car.Y + car.Width / 2);
                            if (temp < distance) distance = temp;
                        }
                        break;
                    case Worldinformation.CIRCUIT_PARTS.PIT:

                        break;
                }
            }
            return distance;
        }
        protected (float, float, float) DistanceToSpeedAndHPOfOpponentInFront()
        {
            float distance = float.MaxValue;
            Car tempCar = null;
            foreach (Car car in _neighbouringCars)
            {
                int temp;
                switch (WhatPartOfCircuitIsCarOn())
                {
                    case Worldinformation.CIRCUIT_PARTS.RIGHT_TURN:

                        break;
                    case Worldinformation.CIRCUIT_PARTS.LEFT_TURN:


                        break;
                    case Worldinformation.CIRCUIT_PARTS.TOP:
                        if (car.X > X) continue;
                        if (Math.Abs(car.Y - Y) > Width / 2 + car.Width / 2) continue;
                        temp = (int)((X - Length / 2) - (car.X + car.Length / 2));
                        if (temp < distance)
                        {
                            distance = temp;
                            tempCar = car;
                        }
                        break;
                    case Worldinformation.CIRCUIT_PARTS.BOTTOM:
                        if (car.X < X) continue;
                        if (Math.Abs(car.Y - Y) > Width / 2 + car.Width / 2) continue;
                        temp = (int)((car.X - car.Length / 2) - (X + Length / 2));
                        if (temp < distance)
                        {
                            distance = temp;
                            tempCar = car;
                        }
                        break;
                    default:
                        break;
                }
            }
            return (distance, (tempCar != null) ? tempCar.Speed : float.MaxValue, (tempCar != null) ? tempCar.CurrentHorsePower : float.MaxValue);
        }
        private float CalculateEnteringAngle(bool rightTurnControl)
        {
            if (rightTurnControl)
            {
                if (Y < _worldInf.CanvasCenterY)
                {
                    //TOP RIGHT
                    double alpha = Math.Asin(Math.Abs(Y - _rightCircle.Y) / _circleRadius);
                    return (float)alpha;
                }
                else
                {
                    //BOTTOM RIGHT
                    double alpha = Math.Asin(Math.Abs(X - _rightCircle.X) / _circleRadius);
                    return (float)(alpha - Math.PI / 2);
                }
            }
            else
            {
                if (Y < _worldInf.CanvasCenterY)
                {
                    //TOP LEFT
                    double alpha = Math.Asin(Math.Abs(X - _leftCircle.X) / _circleRadius);
                    return (float)(alpha + Math.PI / 2);
                }
                else
                {
                    //BOTTOM LEFT
                    double alpha = Math.Asin(Math.Abs(Y - _leftCircle.Y) / _circleRadius);
                    return (float)(alpha + Math.PI);
                }
            }
        }
        public Worldinformation.CIRCUIT_PARTS WhatPartOfCircuitIsCarOn()
        {
            if (X < _leftCircle.X) return Worldinformation.CIRCUIT_PARTS.LEFT_TURN;
            if (X > _rightCircle.X) return Worldinformation.CIRCUIT_PARTS.RIGHT_TURN;
            if (Y < _worldInf.CanvasCenterY) return Worldinformation.CIRCUIT_PARTS.TOP;
            if (Math.Abs(Y - _worldInf.PitPosY) < _worldInf.PenCircuitSize / 4) return Worldinformation.CIRCUIT_PARTS.PIT;
            return Worldinformation.CIRCUIT_PARTS.BOTTOM;
        }

    }
}