using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Nascar_Races_Client
{
    internal class ClientTCPHandler
    {
        private static int _dataPort { get; } = 2000;
        private static int _commPort { get; } = 2001;
        private static string _serverIP { get; } = "127.0.0.1";
        private TcpClient _dataClient;
        private TcpClient _commClient;
        private NetworkStream _dataStream;
        private NetworkStream _commStream;
        public ClientTCPHandler()
        {
            if (Connect())
            {

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

        private void Dispose()
        {
            _dataClient.Dispose();
            _commClient.Dispose();
        }
    }
}
