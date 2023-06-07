using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace NASCAR_Races_Server
{
    internal class CarClientHandler
    {
        private static int startRaceSignal { get; } = 420;
        private TcpClient _dataClient;
        private TcpClient _commClient;
        private NetworkStream _dataStream;
        private NetworkStream _commStream;

        private Car _myCar;
        private Thread _dataThread;
        private Thread _commThread;
        private BinaryFormatter _binaryFormatter;
        private int _dataLength = 919;
        public CarClientHandler(TcpClient dataClient, TcpClient commClient, int myCarNumber)
        {
            _dataClient = dataClient;
            _dataStream = _dataClient.GetStream();

            _commClient = commClient;
            _commStream = _commClient.GetStream();

            _myCar = new();

            _binaryFormatter = new BinaryFormatter();

            _dataThread = new(ExchangeData);
            _dataThread.Start();

            _commThread = new(ExchangeComm);
            //_commThread.Start();
            SendComm(myCarNumber);
        }
        private void ExchangeData()
        {
            while (true)
            {
                byte[] response = new byte[_dataLength];
                int bytesRead = _dataStream.Read(response, 0, _dataLength);
                var temp = DeserializeData(response);
                _myCar.CopyMapper(temp);
            }
        }
        private void ExchangeComm()
        {
            while (true)
            {

            }
        }
        private void SendComm(int signal)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, signal);
                _commStream.Write(ms.ToArray());
                Debug.WriteLine(ms.ToArray().Length);
            }
        }
        private CarMapper DeserializeData(byte[] response)
        {
            using(MemoryStream memoryStream = new MemoryStream(response))
            {
                var formatter = new BinaryFormatter();
                var myobject = (CarMapper)formatter.Deserialize(memoryStream);
                return myobject;
            }
        }
        public Car GetCar()
        {
            return _myCar;
        }
        public void Start()
        {
            SendComm(startRaceSignal);
        }
    }
}
