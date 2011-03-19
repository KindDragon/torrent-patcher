using System;

namespace TorrentUtilities
{   
	/// <summary>
	/// Class to store dictionary values
	/// </summary>
	public class TVal
	{
		DataType _type;
		object _object;

		public TVal(DataType DType, object DObject = null)
		{
			_type=DType;
			_object=DObject;
		}

		public DataType Type
		{
			get { return _type; }
		}

		public object dObject
		{
			get { return _object; }
		}

		public string GetImageKey()
		{
			switch (_type)
			{
				case DataType.Int:
					return "i";
				case DataType.List:
					return "l";
				case DataType.Dictionary:
					return "d";
				default:
					return "s";
			}
		}

		public string GetValueString()
		{
			switch (_type)
			{
				case DataType.Int:
					{
						long val = (long)_object;
						return val.ToString();
					}
				case DataType.Byte:
					{
						string str = "";
						byte[] buffer = (byte[])_object;
						string val = BitConverter.ToString(buffer).Replace("-", "");
						str = (buffer.Length > 100) ? (val.Substring(0, 100) + "...") : val;
						return str;
					}
				case DataType.String:
					{
						string str = (string)_object;
						return str;
					}
				default:
					return "";
			}
		}

		public override string ToString()
		{
			string type = "(" + GetImageKey() + ")";
			switch (_type)
			{
				case DataType.Int:
					{
						long val = (long)_object;
						return type + "=" + val.ToString();
					}

				case DataType.Byte:
					{
						string str = "";
						byte[] buffer = (byte[])_object;
						string val = BitConverter.ToString(buffer).Replace("-", "");
						str = (buffer.Length > 100) ? (val.Substring(0, 100) + "...") : val;
						string[] strArray = new string[] { type, "[", buffer.Length.ToString(), "]=", str };
						return string.Concat(strArray);
					}
				case DataType.String:
					{
						string str = (string)_object;
						return type + "[" + str.Length.ToString() + "]=" + str;
					}
				default:
					return type;
			}
		}
	}
}

