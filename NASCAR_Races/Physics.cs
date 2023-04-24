using System;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace NASCAR_Races
{
    public class Physics
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Length { get; private set; } = 15;
        public float Width { get; private set; } = 10;
        public float Speed { get; private set; } = 0;
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
        private System.DateTime _lastExecutionTime;

        protected Point _leftCircle { get; set; }
        protected Point _rightCircle { get; set; }
        protected int _circleRadius { get; set; }

        protected Point _leftPerfectCircle { get; }
        protected Point _rightPerfectCircle { get; }
        protected int _perfectCircleRadius { get; }

        protected bool _perfectCircle { get; set; }

        private float _turnRadius;
        private float _UseOftires = 0.5f;


        protected float GasPedalDepression;

        protected float FuelMass;
        protected float FuelCapacity;
        protected float FuelBurningRatio = 0.5f;

        protected float MaxHorsePower;
        protected float CurrentHorsePower = 55560; // Force should be in the Future
        protected float CurrentHorsePowerCopy = 55560; // Force should be in the Future
        protected float BrakesForce = 50000;

        protected List<Car> _neighbouringCars;



        private double currentTurnAngle = -Math.PI / 2;

        private Worldinformation _worldInf;

        //LOGS
        public bool isbraking = false;

        public Physics() { }
        public Physics(float x, float y, float mass, float fuelCapacity, float frictionofweels, Worldinformation worldInfo)
        {
            X= x;
            Y= y;
            _mass = mass;
            _turnRadius = worldInfo.TurnRadius;
            FuelMass = fuelCapacity;
            FuelCapacity = fuelCapacity;
            _frictionofweels = frictionofweels;
            _lastExecutionTime = DateTime.Now;

            _worldInf = worldInfo;

            List<Point>temp = worldInfo.PerfectTurnCirclePoints(false);
            List<double> temp2 = FindCircle(temp);
            _leftPerfectCircle = new Point((int)temp2[0], (int)temp2[1]);
            _leftCircle = _leftPerfectCircle;
            _perfectCircleRadius = (int)temp2[2];
            _circleRadius = _perfectCircleRadius;
            temp = worldInfo.PerfectTurnCirclePoints();
            temp2 = FindCircle(temp);
            _rightPerfectCircle=new Point((int)temp2[0], (int)temp2[1]);
            _rightCircle = _rightPerfectCircle;
        }
        // Run in the loop
        public void RunPhysic()
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeSinceLastExecution = currentTime - _lastExecutionTime;
            //float FF = FrictionForce(); // siła która przeciwdziała sile dośrodkowej
            float timeTemp = (float)timeSinceLastExecution.TotalSeconds;
            float wheelFriction = 60;
            float AirR = AirResistance();
            _currentAcceleration = (AccelerationForce() - AirR - wheelFriction) / _mass;
            Speed += (float)(_currentAcceleration * timeTemp);
                
            //if (Acceleration() < AirR + wheelFriction) _currentAcceleration = 0;
            //FuelMass -= CurrentHorsePower * FuelBurningRatio; // * time

            if (X > _rightCircle.X)
            {
                MoveCarOnCircle((float)timeSinceLastExecution.TotalSeconds, true, _rightCircle);
            }
            else if(X < _leftCircle.X)
            {
                MoveCarOnCircle((float)timeSinceLastExecution.TotalSeconds, true, _leftCircle);
            }
            else
            {
                MoveCarOnStraight((float)timeSinceLastExecution.TotalSeconds);
                /*if (Y <= _worldInf.CanvasCenterY)
                {
                    X -= Speed * (float)timeSinceLastExecution.TotalSeconds;
                    HeadingAngle = 0;

                    if (X - _leftPerfectCircle.X <= 10 && (IscentrifugalForce(IscentrifugalForce(_perfectCircleRadius)) != 0))
                    {
                        //_currentAcceleration -= (BrakingForce()/_mass);
                        CurrentHorsePower = 0;
                    }
                    else { CurrentHorsePower = 55560; }

                }
                else
                {
                    X += Speed * (float)timeSinceLastExecution.TotalSeconds;
                    HeadingAngle = 180;

                    if (X - _rightPerfectCircle.X <= 10 && (IscentrifugalForce(IscentrifugalForce(_perfectCircleRadius)) != 0))
                    {
                        //_currentAcceleration -= (BrakingForce() / _mass);
                        CurrentHorsePower = 0;
                    }
                    else { CurrentHorsePower = 55560; }


                }*/
            }

            _lastExecutionTime = currentTime;
            //WriteLogs();
        }
        private void Braking(float timeTemp)
        {
            isbraking = true;
            CurrentHorsePower = 0;
            Speed -= timeTemp * BrakingForce() / _mass;
        }
        private void notBraking()
        {
            isbraking = false;
            CurrentHorsePower = CurrentHorsePowerCopy;
        }
        private void MoveCarOnCircle(float timeElapsed, bool rightCircleControll, Point circle)
        {
            //distanceToEndOfTheTrack = positionofcar - positionofBorder
            //float r = DistanceFromPointToPoint(X, Y, circle.X, circle.Y);
            float r = _circleRadius;

            // Wyznaczamy nowy kąt, uwzględniając czas i prędkość
            float a=0, b=0;
            if(rightCircleControll)
            {
                a= circle.X;
                b= circle.Y;
            }
            if(IscentrifugalForce(_circleRadius)!= 0)
            {
                Braking(timeElapsed);
            }
            else
            {
                notBraking();
            }
            currentTurnAngle += Speed *timeElapsed/ r;
            HeadingAngle =-(float)((currentTurnAngle+Math.PI/2) * (180.0 / Math.PI));
            // Wyznaczamy nowe współrzędne X i Y samochodu
            X = a + r * (float)Math.Cos(-currentTurnAngle);
            Y = b + r * (float)Math.Sin(-currentTurnAngle);

            //return new Tuple<float, float>(x, y);
        }
        private void MoveCarOnStraight(float timeElapsed)
        {
            if (Y < _worldInf.CanvasCenterY)
            {
                //TOP
                X -= Speed * timeElapsed;
                HeadingAngle = 0;
                int enteringPoint = _leftCircle.Y - _circleRadius;
                if (Y != enteringPoint)
                {
                    Y = (Y < enteringPoint) ? Y += 1f : Y -= 1f;
                    if(Math.Abs(Y-enteringPoint)<1)Y=enteringPoint;
                }
            }
            else
            {
                //BOTTOM
                X += Speed * timeElapsed;
                HeadingAngle = 180;
                /*int enteringPoint = _rightCircle.Y + _circleRadius;
                if (Y != enteringPoint)
                {
                    Y = (Y < enteringPoint) ? Y += 1f : Y -= 1f;
                    if(Math.Abs(Y-enteringPoint)<1)Y=enteringPoint;
                }*/
                if (DistanceToOpponentOnRight() > 1)
                {
                    Y += 1f;
                }
            }

            if (IscentrifugalForce(_circleRadius) != 0 && ((_leftPerfectCircle.Y > Y && X < _leftPerfectCircle.X + _circleRadius/2) || (_rightPerfectCircle.Y < Y && X> _rightPerfectCircle.X - _circleRadius /2)))
            {
                Braking(timeElapsed);
            }
            else
            {
                notBraking();
            }
        }
        
        public float DistanceFromPointToPoint(float x1,float y1, float x2, float y2)
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
        private float BrakingForce()
        {
            return BrakesForce * _UseOftires;
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
        public static List<double> FindCircle(List<Point> points)
        {
            if(points.Count == 3)
                return FindCircle(points[0].X, points[0].Y, points[1].X, points[1].Y, points[2].X, points[2].Y);
            return FindCircle(points[0].X, points[0].Y, points[1].X, points[1].Y);
        }
        //returns List where:
        //List[0] - X coordinate of Circle Center
        //List[1] - Y coordinate of Circle Center
        //List[2] - Radius of Circle
        private static List<double> FindCircle(int x1, int y1,
                                        int x2, int y2,
                                        int x3, int y3)
        {
            double x12 = x1 - x2;
            double x13 = x1 - x3;

            double y12 = y1 - y2;
            double y13 = y1 - y3;

            double y31 = y3 - y1;
            double y21 = y2 - y1;

            double x31 = x3 - x1;
            double x21 = x2 - x1;

            // x1^2 - x3^2
            double sx13 = (int)(Math.Pow(x1, 2) -
                            Math.Pow(x3, 2));

            // y1^2 - y3^2
            double sy13 = (int)(Math.Pow(y1, 2) -
                            Math.Pow(y3, 2));

            double sx21 = (int)(Math.Pow(x2, 2) -
                            Math.Pow(x1, 2));

            double sy21 = (int)(Math.Pow(y2, 2) -
                            Math.Pow(y1, 2));

            double f = ((sx13) * (x12)
                    + (sy13) * (x12)
                    + (sx21) * (x13)
                    + (sy21) * (x13))
                    / (2 * ((y31) * (x12) - (y21) * (x13)));
            double g = ((sx13) * (y12)
                    + (sy13) * (y12)
                    + (sx21) * (y13)
                    + (sy21) * (y13))
                    / (2 * ((x31) * (y12) - (x21) * (y13)));

            double c = -Math.Pow(x1, 2) - Math.Pow(y1, 2) -
                                        2 * g * x1 - 2 * f * y1;

            // eqn of circle be x^2 + y^2 + 2*g*x + 2*f*y + c = 0
            // where centre is (h = -g, k = -f) and radius r
            // as r^2 = h^2 + k^2 - c
            double h = -g;
            double k = -f;
            double sqr_of_r = h * h + k * k - c;

            // r is the radius
            double r = Math.Round(Math.Sqrt(sqr_of_r), 5);

            List<double> result = new List<double>();
            result.Add(h);
            result.Add(k);
            result.Add(r);
            return result;
        }
        //works only when x1=x2
        protected static List<double> FindCircle(int x1, int y1,
                                        int x2, int y2)
        {
            double centerX = x1;
            double centerY = (y1 + y2) / 2.0;
            double radius = Math.Abs(y1 - y2) / 2.0;
            List<double> res = new List<double>();
            res.Add(centerX);
            res.Add(centerY);
            res.Add(radius);
            return res;
        }
        protected float DistanceToOpponentOnRight()
        {
            float distance = _worldInf.PenCircuitSize;
            foreach(Car car in _neighbouringCars)
            {
                List<float> myCoordintes;
                List<float> opponentCoordinates;
                myCoordintes = getRightSideCoordinates();
                opponentCoordinates = car.getLeftSideCoordinates();
                switch (_worldInf.WhatPartOfCircuitIsCarOn(this))
                {
                    case Worldinformation.CIRCUIT_PARTS.LEFT_TURN:

                        break;
                    case Worldinformation.CIRCUIT_PARTS.RIGHT_TURN:

                        break;
                    case Worldinformation.CIRCUIT_PARTS.TOP:
                        //Car will enter "left" turn
                        
                        break;
                    case Worldinformation.CIRCUIT_PARTS.BOTTOM:
                        //Car will enter "right" turn
                        
                        

                        break;
                    case Worldinformation.CIRCUIT_PARTS.PIT:
                        



                        break;
                }
            }
            return 0;
        }
        //return:
        //List[0]=Y coordinate
        //List[1]=X1 coordinate
        //List[2]=X2 coordinate
        public List<float> getRightSideCoordinates()
        {
            var partOfTrack = _worldInf.WhatPartOfCircuitIsCarOn(this);
            List<float>res=new List<float>();
            if (partOfTrack == Worldinformation.CIRCUIT_PARTS.BOTTOM)
            {
                res.Add(Y-Width/2);
                res.Add(X - Length / 2);
                res.Add(X + Length / 2);
            }else if(partOfTrack==Worldinformation.CIRCUIT_PARTS.TOP)
            {
                res.Add(Y + Width / 2);
                res.Add(X - Length / 2);
                res.Add(X + Length / 2);
            }
            return res;
        }
        public List<float> getLeftSideCoordinates()
        {
            var partOfTrack = _worldInf.WhatPartOfCircuitIsCarOn(this);
            List<float> res = new List<float>();
            if (partOfTrack == Worldinformation.CIRCUIT_PARTS.BOTTOM)
            {
                res.Add(Y + Width / 2);
                res.Add(X - Length / 2);
                res.Add(X + Length / 2);
            }
            else if (partOfTrack == Worldinformation.CIRCUIT_PARTS.TOP)
            {
                res.Add(Y - Width / 2);
                res.Add(X - Length / 2);
                res.Add(X + Length / 2);
            }
            return res;
        }
    }
}