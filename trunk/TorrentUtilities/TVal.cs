using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace TorrentUtilities
{
	/// <summary>
	/// The Data types
	/// </summary>
	public enum DataType
	{
		String,
		Int,
		List,
		Dictionary,
		Byte
	}

	/// <summary>
	/// Class to store dictionary values
	/// </summary>
	public class TVal
	{
		DataType _type;
		object _object;
		Encoding _encoding;

		public TVal(DataType DType, object DObject, Encoding DEncoding = null)
		{
			_type=DType;
			_object=DObject;
			_encoding=DEncoding;
		}

		public TVal(long value)
			: this(DataType.Int, value)
		{
		}

		public TVal(byte[] value)
			: this(DataType.Byte, value)
		{
		}

		public TVal(string value, Encoding DEncoding = null)
			: this(DataType.String, value, DEncoding)
		{
		}

		public TVal(Dictionary<string, TVal> value)
			: this(DataType.Dictionary, value)
		{
		}

		public TVal(List<TVal> value)
			: this(DataType.List, value)
		{
		}

		public static explicit operator long(TVal val)
		{
			Debug.Assert(val._type == DataType.Int);
			return (long)val._object;
		}

		public static explicit operator byte[](TVal val)
		{
			Debug.Assert(val._type == DataType.Byte);
			return (byte[])val._object;
		}

		public static explicit operator string(TVal val)
		{
			Debug.Assert(val._type == DataType.String);
			return (string)val._object;
		}

		public static explicit operator Dictionary<string, TVal>(TVal val)
		{
			Debug.Assert(val._type == DataType.Dictionary);
			return (Dictionary<string, TVal>)val._object;
		}

		public static explicit operator List<TVal>(TVal val)
		{
			Debug.Assert(val._type == DataType.List);
			return (List<TVal>)val._object;
		}

		public DataType Type
		{
			get { return _type; }
		}

		public object dObject
		{
			get { return _object; }
		}

		public Encoding dEncoding
		{
			get { return _encoding; }
		}

		public string GetTypeStr()
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
			string type = "(" + GetTypeStr() + ")";
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

