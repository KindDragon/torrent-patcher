using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace TorrentPatcher
{
	/// <summary>
	/// Class for geotargeting using ipgeobase.ru XML-service
	/// v0.1 by Kiss_Lee_Zin (http://www.last.fm/music/insanity13)
	/// </summary>
	public class GEOIP
	{
		#region "Properties"

		public static bool UseUTF8 { get; set; }

		/// <summary>
		/// Use Moscow info on request error
		/// </summary>
		public static bool UseDefaultCity { get; set; }

		/// <summary>
		/// Use proxy Authentication
		/// </summary>
		public static bool UseAuthentification { get; set; }

		public static string ProxyName { get; set; }

		public static int ProxyPort { get; set; }

		public static string CredentialsName { get; set; }

		public static string CredentialsPass { get; set; }

		/// <summary>
		/// ipgeobase XML Service address
		/// </summary>
		private static string _ServiceAddress;
		public static string ServiceAddress
		{
			get
			{
				if (string.IsNullOrEmpty(_ServiceAddress)) _ServiceAddress = "http://194.85.91.253:8090/geo/geo.html";
				return _ServiceAddress;
			}
			set { _ServiceAddress = value; }
		}

		#endregion

		public struct IPInfo
		{
			public string city;
			public string region;
			public string district;
			public string lat;
			public string lng;
			public string inetNum;
			public string inetDescr;
			public string inetStatus;
			public string ip;
		}

		/// <summary>
		/// Create IPInfo query for single IP
		/// </summary>
		/// <param name="ip">IP-Address</param>
		/// <returns>XML query</returns>
		public static string createRequest(string ip)
		{
			return createRequest(new String[1] { ip });
		}

		/// <summary>
		/// Create IPInfo query for IP list
		/// </summary>
		/// <param name="ip">IP-Address</param>
		/// <returns>XML query</returns>
		public static string createRequest(string[] ipList)
		{

			StringBuilder request = new StringBuilder();
			//Create XML query
			request.Append("<ipquery><fields><all/></fields><ip-list>");
			foreach (string ip in ipList)
			{
				//IP Validation.
				IPAddress.Parse(ip);
				request.AppendFormat("<ip>{0}</ip>", ip);
			}
			request.Append("</ip-list></ipquery>");
			return request.ToString();
		}

		private static string GetNodeValue(XmlNode Node, string xpath)
		{
			XmlNode n = Node.SelectSingleNode(xpath);
			return n != null ? n.InnerText : null;
		}

		public static Nullable<IPInfo> getSingleIPInfo(string ip)
		{
			XmlDocument xDocument = new XmlDocument();
			XmlNode Node;
			IPInfo IPInfo;

			xDocument.InnerXml = getXMLResponse(createRequest(ip));

			Node = xDocument.SelectSingleNode(string.Format("/ip-answer/ip[@value='{0}']", ip));

			if (Node == null) return null;
			IPInfo.ip = Node.Attributes["value"].Value;
			IPInfo.city = GetNodeValue(Node, "city");
			IPInfo.region = GetNodeValue(Node, "region");
			IPInfo.district = GetNodeValue(Node, "district");
			IPInfo.lat = GetNodeValue(Node, "lat");
			IPInfo.lng = GetNodeValue(Node, "lng");
			IPInfo.inetNum = GetNodeValue(Node, "inetnum");
			IPInfo.inetDescr = GetNodeValue(Node, "inet-descr");
			IPInfo.inetStatus = GetNodeValue(Node, "inet-status");
			return IPInfo;
		}

		public static Dictionary<string, IPInfo> getIPInfo(string[] ipList)
		{
			string Request = createRequest(ipList);
			XmlDocument xDocument = new XmlDocument();
			xDocument.InnerXml = getXMLResponse(Request);
			return getIPInfo(xDocument);
		}


		public static Dictionary<string, IPInfo> getIPInfo(XmlDocument xDocument)
		{
			IPInfo IPInfo;
			Dictionary<string, IPInfo> IPInfoList = new Dictionary<string, IPInfo>();

			foreach (XmlNode Node in xDocument.SelectNodes("/ip-answer/ip"))
			{
				IPInfo.ip = Node.Attributes["value"].Value;
				IPInfo.city = GetNodeValue(Node, "city");
				IPInfo.region = GetNodeValue(Node, "region");
				IPInfo.district = GetNodeValue(Node, "district");
				IPInfo.lat = GetNodeValue(Node, "lat");
				IPInfo.lng = GetNodeValue(Node, "lng");
				IPInfo.inetNum = GetNodeValue(Node, "inetnum");
				IPInfo.inetDescr = GetNodeValue(Node, "inet-descr");
				IPInfo.inetStatus = GetNodeValue(Node, "inet-status");
				IPInfoList.Add(IPInfo.ip, IPInfo);
			}

			return IPInfoList;
		}

		public static XmlDocument getXMLResponse(XmlDocument Request)
		{
			XmlDocument xDocument = new XmlDocument();
			xDocument.InnerXml = getXMLResponse(Request.InnerXml);
			return xDocument;
		}

		/// <summary>
		/// Make a request
		/// </summary>
		/// <param name="Request">XMLtype IPInfo request</param>
		/// <returns></returns>
		public static String getXMLResponse(string Request)
		{
			//Create obj for request
			HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(ServiceAddress);
			//Set query type
			objRequest.Method = "POST";
			objRequest.ContentType = "application/xml";
			if (UseAuthentification)
			{
				objRequest.PreAuthenticate = true;
				objRequest.Proxy = new WebProxy(ProxyName, ProxyPort);
				objRequest.Proxy.Credentials = new NetworkCredential(CredentialsName, CredentialsPass);
			}

			//Open stream
			using (Stream str = objRequest.GetRequestStream())
			{
				//send request
				using (XmlWriter writer = XmlWriter.Create(str))
				{
					writer.WriteStartDocument();
					writer.WriteStartElement("request");
					writer.WriteRaw(Request);
					writer.WriteEndElement();
					writer.WriteEndDocument();
				}
			}

			string allReq = "";
			StreamReader reader;
			try
			{
				//Getting response
				HttpWebResponse myResponse = (HttpWebResponse)objRequest.GetResponse();
				if (UseUTF8) reader = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
				else reader = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("Windows-1251"));
				allReq = reader.ReadToEnd();
				reader.Close();
			}
			catch (Exception ex)
			{
				if (UseDefaultCity)
					allReq = "<?xml version='1.0' encoding='Windows-1251'?><ip-answer>\n<ip value=''><inetnum/><inet-descr/><inet-status/><city>Москва</city><region>Москва</region><district>Центральный</district><lat>55.755787</lat><lng>37.617634</lng></ip>\n</ip-answer>";
				else
					throw ex;
				/*Do something*/
			}
			return allReq;
		}
	}
}
