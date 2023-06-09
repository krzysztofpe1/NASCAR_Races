﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NASCAR_Races_Server;
using System.Security.Cryptography.X509Certificates;

namespace Nascar_Races_Client
{
    public class Car : Physics
    {
        public bool IsDisposable { get; set; } = false;
        public bool Started { get; set; } = false;

        public string CarName { get; private set; }

        private WorldInformation _worldInfo;
        public Car(Point startingPos, Point pitPos, float weight, string carName, float maxHorsePower, WorldInformation worldInfo) : base(startingPos.X, startingPos.Y, weight, 0.3f, maxHorsePower, worldInfo)
        {
            CarName = carName;
            _worldInfo = worldInfo;
            _pitPos = pitPos;
        }
        Random random = new Random();
        public void Move()
        {
            _rightCircle = new Point(int.MaxValue, int.MaxValue);
            _leftCircle = new Point(0, 0);
            int counter = 0;
            while (!IsDisposable)
            {
                if (!Started)
                {
                    _lastExecutionTime = DateTime.Now;
                    continue;
                }
                //refreshing neighbouring cars list every 10 iterations
                if (++counter >= 5)
                //if (true)
                {
                    counter = 0;
                    var partOfCircuit = WhatPartOfCircuitIsCarOn();
                    int distanceToOpponentOnLeft = (int)DistanceToOpponentOnLeft();
                    switch (partOfCircuit)
                    {
                        case WorldInformation.CIRCUIT_PARTS.LEFT_TURN:

                            break;
                        case WorldInformation.CIRCUIT_PARTS.RIGHT_TURN:

                            break;
                        case WorldInformation.CIRCUIT_PARTS.TOP:
                            //Car will enter "left" turn
                            if (NeighbouringCars.Count > 0) FindSafeCircle((int)Y, false);
                            else FindCircle((int)Y, false);
                            break;
                        case WorldInformation.CIRCUIT_PARTS.BOTTOM:
                            //Car will enter "right" turn
                            if (NeighbouringCars.Count > 0) FindSafeCircle((int)Y, true);
                            else FindCircle((int)Y, true);
                            break;
                        case WorldInformation.CIRCUIT_PARTS.PIT:
                            State = STATE.PIT;
                            FindSafeCircle((int)Y, true);
                            break;
                    }
                }
                RunPhysic();
                Thread.Sleep(10);
            }
        }

        public CarMapper CreateMap()
        {
            //carMapper.
            var carMapper = new CarMapper
            {
                //CAR variables
                IsDisposable = this.IsDisposable,
                CarName = this.CarName,
            };
            return this.MapPhysics(carMapper);
        }

        public void CopyMapper(CarMapper map)
        {
            IsDisposable = map.IsDisposable;
            CarName = map.CarName;
            base.CopyMapper(map);
        }
    }
}
