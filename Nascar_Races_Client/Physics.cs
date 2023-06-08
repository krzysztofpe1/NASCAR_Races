using System;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.VisualBasic.Logging;
using NASCAR_Races_Server;

namespace Nascar_Races_Client
{
    public class Physics : DrawableCar
    {
        const float accelerationOfGravity = 9.81f;
        const float trackAngle = 0.175f;
        const float airDensity = 1.225f;
        const float frontSurface = 2.5f;
        const float carAirDynamic = 0.35f;
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

        //change to private for logs only
        public float _currentAcceleration { get; private set; }
        private float _mass { get; set; }
        private float _frictionofweels { get; set; }
        protected System.DateTime _lastExecutionTime { get; set; }

        protected Point _leftCircle { get; set; }
        protected Point _rightCircle { get; set; }
        protected int _circleRadius { get; set; }
        protected Point _pitPos { get; set; }

        public float FuelMass { get; private set; }
        protected float FuelBurningRatio { get; } = 0.00001f;

        public float MaxHorsePower { get; set; }
        public float CurrentHorsePower { get; set; }

        protected List<Car> _neighbouringCars { get; set; }

        protected bool _recalculateHeadingAngle { get; set; } = true;

        private double currentTurnAngle { get; set; } = -Math.PI / 2;

        private WorldInformation _worldInf { get; set; }
        private int _carSafeDistance { get; set; }

        public enum STATE
        {
            ON_CIRCUIT,
            OFF_CIRCUIT,
            ON_WAY_TO_PIT_STOP,
            PIT,
            PIT_STOPPED
        }

        public STATE State { get; protected set; } = STATE.ON_CIRCUIT;

        public Physics() { }

        public Physics(float x, float y, float mass, float frictionofweels, float maxHorsePower, WorldInformation worldInfo)
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
            //timeSinceLastExecution *= 2;
            //float FF = FrictionForce(); // siła która przeciwdziała sile dośrodkowej
            float wheelFriction = 60;
            float airR = AirResistance();
            _currentAcceleration = (AccelerationForce() - airR - wheelFriction) / _mass;
            Speed += (float)(_currentAcceleration * (float)timeSinceLastExecution.TotalSeconds);

            //if (Acceleration() < AirR + wheelFriction) _currentAcceleration = 0;
            //FuelMass -= CurrentHorsePower * FuelBurningRatio; // * time
            var partOfCircuit = WhatPartOfCircuitIsCarOn();
            if (partOfCircuit == WorldInformation.CIRCUIT_PARTS.RIGHT_TURN)
            {
                MoveCarOnCircle((float)timeSinceLastExecution.TotalSeconds, true, _rightCircle);
            }
            else if (partOfCircuit == WorldInformation.CIRCUIT_PARTS.LEFT_TURN)
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
        private void MoveCarOnStraight(float timeElapsed, WorldInformation.CIRCUIT_PARTS partOfCircuit)
        {
            CurrentHorsePower = MaxHorsePower;
            if (partOfCircuit == WorldInformation.CIRCUIT_PARTS.TOP)
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
            else if (partOfCircuit == WorldInformation.CIRCUIT_PARTS.BOTTOM)
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
            else if (partOfCircuit == WorldInformation.CIRCUIT_PARTS.PIT)
            {
                float Xtemp = X;
                float Ytemp = Y;
                HeadingAngle = 180;
                //timeElapsed = 0.01f;
                int bottomBorderPit = _worldInf.PitPosY + _worldInf.PenCircuitSize / 4 - (int)Length / 2;
                if (Y < bottomBorderPit && _pitPos.X - 100 > X)
                {
                    Y += 0.25f;
                }
                if (Speed > _worldInf.CarMaxSpeedInPit)
                {
                    CurrentHorsePower = 0;
                    Speed -= 0.5f;
                }
                else
                {
                    CurrentHorsePower = MaxHorsePower;
                    Speed += 0.25f;
                }

                X += Speed * timeElapsed;

                double distanceToPit = Math.Abs(_pitPos.X - X);
                if (distanceToPit < _worldInf.CarLengthOfPittingManouver - 2 && X < _pitPos.X)
                {
                    //AcrTan in <-2;2> interval; Value: <Atan(-2);Atan(2)>
                    double temp = Math.Atan(((double)_worldInf.CarLengthOfPittingManouver / 2 - distanceToPit) / (double)_worldInf.CarLengthOfPittingManouver * 4);
                    //Value: <0;Atan(2)>
                    temp -= Math.Atan(-2);
                    //Value: <0;1>
                    temp /= Math.Atan(2);
                    //Value: <0;_worldInf.CarWidthOfPittingManouver)
                    temp *= _worldInf.CarWidthOfPittingManouver;
                    //Debug.WriteLine(temp);
                    Y = (float)(bottomBorderPit - temp);
                }
                if (distanceToPit <= 2 && distanceToPit >= -2)
                {
                    State = STATE.PIT_STOPPED;
                    FuelMass = _worldInf.CarInitialFuelMass;
                    Thread.Sleep(1000);
                    X += 4;
                    _lastExecutionTime = DateTime.Now;
                }
                else if (X > _pitPos.X && distanceToPit + _worldInf.CarLengthOfPittingManouver > 0)
                {
                    //AcrTan in <-2;2> interval; Value: <Atan(-2);Atan(2)>
                    double temp = Math.Atan(((double)_worldInf.CarLengthOfPittingManouver / 2 - distanceToPit) / (double)_worldInf.CarLengthOfPittingManouver * 4);
                    //Value: <0;Atan(2)>
                    temp -= Math.Atan(-2);
                    //Value: <0;1>
                    temp /= Math.Atan(2);
                    //Value: <0;_worldInf.CarWidthOfPittingManouver)
                    temp *= _worldInf.CarWidthOfPittingManouver;
                    //Debug.WriteLine(temp);
                    Y = (float)(bottomBorderPit - temp);
                }
                HeadingAngle = 180 + (float)Math.Atan((Ytemp - Y) / (Xtemp - X)) * 20;
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
                    case WorldInformation.CIRCUIT_PARTS.LEFT_TURN:

                        break;
                    case WorldInformation.CIRCUIT_PARTS.RIGHT_TURN:

                        break;
                    case WorldInformation.CIRCUIT_PARTS.TOP:
                        //Car will enter "left" turn
                        if (car.Y < Y)
                        {
                            //opponent is on the right side of this car
                            temp = (Y - Width / 2) - (car.Y + car.Width / 2);
                            if (temp < distance) distance = temp;
                        }
                        break;
                    case WorldInformation.CIRCUIT_PARTS.BOTTOM:
                        //Car will enter "right" turn
                        if (car.Y > Y)
                        {
                            //opponent is on the right side of this car
                            temp = (car.Y - car.Width / 2) - (Y + Width / 2);
                            if (temp < distance) distance = temp;
                        }
                        break;
                    case WorldInformation.CIRCUIT_PARTS.PIT:

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
                    case WorldInformation.CIRCUIT_PARTS.LEFT_TURN:

                        break;
                    case WorldInformation.CIRCUIT_PARTS.RIGHT_TURN:

                        break;
                    case WorldInformation.CIRCUIT_PARTS.TOP:
                        //Car will enter "left" turn
                        if (car.Y > Y)
                        {
                            //opponent is on the left side of this car
                            temp = car.Y - car.Width / 2 - (Y + Width / 2);
                            if (temp < distance) distance = temp;
                        }
                        break;
                    case WorldInformation.CIRCUIT_PARTS.BOTTOM:
                        //Car will enter "right" turn
                        if (car.Y < Y)
                        {
                            //opponent is on the left side of this car
                            temp = Y - Width / 2 - (car.Y + car.Width / 2);
                            if (temp < distance) distance = temp;
                        }
                        break;
                    case WorldInformation.CIRCUIT_PARTS.PIT:

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
                    case WorldInformation.CIRCUIT_PARTS.RIGHT_TURN:

                        break;
                    case WorldInformation.CIRCUIT_PARTS.LEFT_TURN:


                        break;
                    case WorldInformation.CIRCUIT_PARTS.TOP:
                        if (car.X > X) continue;
                        if (Math.Abs(car.Y - Y) > Width / 2 + car.Width / 2) continue;
                        temp = (int)((X - Length / 2) - (car.X + car.Length / 2));
                        if (temp < distance)
                        {
                            distance = temp;
                            tempCar = car;
                        }
                        break;
                    case WorldInformation.CIRCUIT_PARTS.BOTTOM:
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
        public WorldInformation.CIRCUIT_PARTS WhatPartOfCircuitIsCarOn()
        {
            if (X < _leftCircle.X) return WorldInformation.CIRCUIT_PARTS.LEFT_TURN;
            if (X > _rightCircle.X) return WorldInformation.CIRCUIT_PARTS.RIGHT_TURN;
            if (Y < _worldInf.CanvasCenterY) return WorldInformation.CIRCUIT_PARTS.TOP;
            if (Math.Abs(Y - _worldInf.PitPosY) < _worldInf.PenCircuitSize / 4) return WorldInformation.CIRCUIT_PARTS.PIT;
            return WorldInformation.CIRCUIT_PARTS.BOTTOM;
        }
        public CarMapper MapPhysics(CarMapper car)
        {
            car.X = this.X;
            car.Y = this.Y;
            car.Length = this.Length;
            car.Width = this.Width;
            car.Speed = this.Speed;
            car.HeadingAngle = this.HeadingAngle;
            car.FuelMass = this.FuelMass;
            //car.FuelBurningRatio = this.FuelBurningRatio;
            car.MaxHorsePower = this.MaxHorsePower;
            car.CurrentHorsePower = this.CurrentHorsePower;
            car.State = (CarMapper.STATE)this.State;
            return car;
        }
        public void CopyMapper(CarMapper car)
        {

            this.X = car.X;
            this.Y = car.Y;
            this.Length = car.Length;
            this.Width = car.Width;
            this.Speed = car.Speed;
            this.HeadingAngle = car.HeadingAngle;
            this.FuelMass = car.FuelMass;
            //this.FuelBurningRatio = car.FuelBurningRatio;
            this.MaxHorsePower = car.MaxHorsePower;
            this.CurrentHorsePower = car.CurrentHorsePower;
            this.State = (STATE)car.State;
        }
    }
}