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
		private int _timeout;

		// Public constructor provides an easy way to supply all
		// the information needed for the task.
		public TaskInfo(string Ser, int N, bool Res, ManualResetEvent doneEvent, int timeout = 300)
		{
			_server = Ser;
			_port = N;
			_result = Res;
			_doneEvent = doneEvent;
			_timeout = timeout;
		}

		// Public constructor provides an easy way to supply all
		// the information needed for the task.
		public TaskInfo(string Ser, bool Res, ManualResetEvent doneEvent, int timeout = 300)
		{
			_server = Ser;
			_result = Res;
			_ping = true;
			_doneEvent = doneEvent;
			_timeout = timeout;
		}

		// Wrapper method for use with thread pool.
		public void ThreadPoolCallback(object threadContext)
		{
			int threadIndex = (int)threadContext;
			_result = CheckConnection();
			_doneEvent.Set();
		}

		/// <summary>
		/// Checks connection to current server and port
		/// </summary>
		/// <param name="server">server to connect</param>
		/// <param name="port">port to connect</param>
		/// <returns>connection success or not</returns>
		private bool CheckConnection()
		{
			bool result = false;
			try
			{
				// Get host related information.
				IPAddress[] addressList = Dns.GetHostAddresses(_server);
				if (!_ping)
					result = CheckConnectionUsingSocket(addressList);
				else
					result = CheckConnectionUsingPing(addressList);
			}
			catch (SocketException)
			{
				result = false;
			}
			return result;
		}

		/// <summary>
		/// Checks connection to current server using ping
		/// </summary>
		/// <param name="server">server to connect</param>
		/// <returns>connection success or not</returns>
		private bool CheckConnectionUsingPing(IPAddress[] addressList)
		{
			try
			{
				using (Ping ping = new Ping())
				{
					PingOptions options = new PingOptions();
					// Use the default Ttl value which is 128,
					// but change the fragmentation behavior.
					options.DontFragment = true;
					// Create a buffer of 32 bytes of data to be transmitted.
					string s = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
					byte[] bytes = Encoding.ASCII.GetBytes(s);

					// Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
					// an exception that occurs when the host IP Address is not compatible with the address family
					// (typical in the IPv6 case).
					foreach (IPAddress address in addressList)
					{
						if (ping.Send(_server, _timeout, bytes, options).Status == IPStatus.Success)
						{
							return true;
						}
					}
				}
			}
			catch(PingException)
			{
			}
			return false;
		}

		/// <summary>
		/// Checks connection to current server and port using socket
		/// </summary>
		/// <param name="server">server to connect</param>
		/// <param name="port">port to connect</param>
		/// <returns>connection success or not</returns>
		private bool CheckConnectionUsingSocket(IPAddress[] addressList)
		{
			try
			{
				// Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
				// an exception that occurs when the host IP Address is not compatible with the address family
				// (typical in the IPv6 case).
				foreach (IPAddress address in addressList)
				{
					IPEndPoint remoteEP = new IPEndPoint(address, _port);
					using (Socket socket = new Socket(remoteEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
					{
						socket.Blocking = false;
						try
						{
							socket.Connect(remoteEP);
						}
						catch (SocketException ex)
						{
							if (ex.SocketErrorCode != SocketError.WouldBlock)
							{
								// Handle bad exception
								throw ex;
							}

							// Wait until connected or timeout.
							// SelectWrite: returns true, if processing a Connect,
							// and the connection has succeeded.
							if (socket.Poll(_timeout * 1000, SelectMode.SelectWrite) == true && 
								socket.Connected)
							{
								socket.Close();
								return true;
							}
						}
					}
				}
			}
			catch(SocketException)
			{
			}
			return false;
		}
	}
}
