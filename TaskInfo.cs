using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TorrentPatcher
{
	public class TaskInfo
	{
		// State information for the task.  These members
		// can be implemented as read-only properties, read/write
		// properties with validation, and so on, as required.
		private string _server;
		private int _port;
		private bool _ping = false;
		private bool _result;
		public bool Result { get { return _result; } }
		private ManualResetEvent _doneEvent;

		// Public constructor provides an easy way to supply all
		// the information needed for the task.
		public TaskInfo(string Ser, int N, bool Res, ManualResetEvent doneEvent)
		{
			_server = Ser;
			_port = N;
			_result = Res;
			_doneEvent = doneEvent;
		}

		// Public constructor provides an easy way to supply all
		// the information needed for the task.
		public TaskInfo(string Ser, bool Res, ManualResetEvent doneEvent)
		{
			_server = Ser;
			_result = Res;
			_ping = true;
			_doneEvent = doneEvent;
		}


		// Wrapper method for use with thread pool.
		public void ThreadPoolCallback(object threadContext)
		{
			int threadIndex = (int)threadContext;
			CheckConnection();
			_doneEvent.Set();
		}

		/// <summary>
		/// Checks connection to current server and port
		/// </summary>
		/// <param name="server">server to connect</param>
		/// <param name="port">port to connect</param>
		/// <returns>connection success or not</returns>
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
				// Use the default Ttl value which is 128,
				// but change the fragmentation behavior.
				options.DontFragment = true;
				// Create a buffer of 32 bytes of data to be transmitted.
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
				// Get host related information.
				IPHostEntry hostEntry = Dns.GetHostEntry(_server);

				// Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
				// an exception that occurs when the host IP Address is not compatible with the address family
				// (typical in the IPv6 case).
				foreach (IPAddress address in hostEntry.AddressList)
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
	}
}
