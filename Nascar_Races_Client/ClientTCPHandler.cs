﻿using NASCAR_Races_Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Nascar_Races_Client
{
    internal class ClientTCPHandler
    {
        private static int startRaceSignal { get; } = 420;
        private static int _dataPort { get; } = 2000;
        private static int _commPort { get; } = 2001;
        private static string _serverIP { get; } = "127.0.0.1";
        private TcpClient _dataClient;
        private TcpClient _commClient;
        private NetworkStream _dataStream;
        private NetworkStream _commStream;
        private Thread _dataThread;
        private Thread _commThread;
        public bool IsDisposable { get; private set; } = false;
        private BinaryFormatter _binaryFormatter;
        public Car MyCar { get; private set; }
        public Thread CarThread { get; private set; }
        public ClientTCPHandler(WorldInformation worldInf)
        {
            _binaryFormatter = new BinaryFormatter();
            
            if (Connect())
            {
                byte[] comm = new byte[54];
                _commStream.Read(comm);
                using (var ms = new MemoryStream(comm))
                {
                    var formatter = new BinaryFormatter();
                    int deserialized = (int)formatter.Deserialize(ms);
                    Debug.WriteLine(deserialized);
                    Point startingPos=new Point();
                    Point pitPos = new Point();
                    int temp = 0;
                    while (temp++ < deserialized)
                    {
                        startingPos = RaceManager.NextStartingPoint();
                        pitPos = RaceManager.NextPitPoint();
                    }
                    MyCar = new(startingPos, pitPos, 1000, deserialized.ToString(), 30000, worldInf);
                    CarThread = new(MyCar.Move);
                }
                Debug.Write(" TUTAJ WESZŁO");
                CarThread = new(MyCar.Move);
                CarThread.Start();
                _dataThread = new(ExchangeData);
                _dataThread.Start();
                _commThread = new(ExchangeComm);
                _commThread.Start();
            }
            else
            {

            }
        }

        private bool Connect()
        {
            try
            {
                _dataClient = new TcpClient(_serverIP, _dataPort);
                _dataStream = _dataClient.GetStream();

                _commClient = new TcpClient(_serverIP, _commPort);
                _commStream = _commClient.GetStream();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private void ExchangeData()
        {
            while (!IsDisposable)
            {
                //sending my object to server every iteration
                var myObjectSerialized = SerializeCar();
                _dataStream.Write(myObjectSerialized, 0, myObjectSerialized.Length);
                Debug.WriteLine(myObjectSerialized.Length);

            }
        }
        private void ExchangeComm()
        {
            while (!IsDisposable)
            {
                byte[] comm = new byte[54];
                _commStream.Read(comm);
                using (var ms = new MemoryStream(comm))
                {
                    var formatter = new BinaryFormatter();
                    int deserialized = (int)formatter.Deserialize(ms);
                    if(deserialized == startRaceSignal)
                    {
                        MyCar.Started = true;
                    }
                }
            }
        }
        private byte[] SerializeCar()
        {
            using (MemoryStream stream = new())
            {
                var formatter = new BinaryFormatter();
                CarMapper map = MyCar.CreateMap();
                _binaryFormatter.Serialize(stream, map);
                return stream.ToArray();
            }
        }

        public void Dispose()
        {
            _dataClient.Dispose();
            _commClient.Dispose();
            IsDisposable = true;
            MyCar.IsDisposable = true;
        }
    }
}
