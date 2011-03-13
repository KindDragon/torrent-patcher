using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TorrentPatcher
{
    public class TaskInfo
    {
        private ManualResetEvent _doneEvent;
        private bool _ping;
        private int _port;
        private bool _result;
        private string _server;

        public TaskInfo(string Ser, bool Res, ManualResetEvent doneEvent)
        {
            _server = Ser;
            _result = Res;
            _ping = true;
            _doneEvent = doneEvent;
        }

        public TaskInfo(string Ser, int N, bool Res, ManualResetEvent doneEvent)
        {
            _server = Ser;
            _port = N;
            _result = Res;
            _doneEvent = doneEvent;
        }

        private void CheckConnection()
        {
            if (!_ping)
                _result = CheckConnectionUsingSocket();
            else
                _result = CheckConnectionUsingPing();
        }

        private bool CheckConnectionUsingPing()
        {
            bool result = false;
            try
            {
                Ping ping = new Ping();
                PingOptions options = new PingOptions();
                options.DontFragment = true;
                string s = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                int timeout = 120;
                if (ping.Send(_server, timeout, bytes, options).Status == IPStatus.Success)
                {
                    result = true;
                }
            }
            catch (NetworkInformationException)
            {
                result = false;
            }
            return result;
        }

        private bool CheckConnectionUsingSocket()
        {
            bool result = false;
            try
            {
                foreach (IPAddress address in Dns.GetHostEntry(_server).AddressList)
                {
                    IPEndPoint remoteEP = new IPEndPoint(address, _port);
                    Socket socket = new Socket(remoteEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(remoteEP);
                    if (socket.Connected)
                    {
                        result = true;
                        socket.Close();
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public void ThreadPoolCallback(object threadContext)
        {
            int num1 = (int)threadContext;
            CheckConnection();
            _doneEvent.Set();
        }

        public bool Result
        {
            get
            {
                return _result;
            }
        }
    }
}
