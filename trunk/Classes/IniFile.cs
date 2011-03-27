using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TorrentPatcher
{
	/// <summary>
	/// Create a New INI file to store or load data
	/// </summary>
	public sealed class IniFile
	{
		private string path;

		static class NativeMethods
		{
			[DllImport("kernel32", SetLastError = true)]
			public static extern int WritePrivateProfileString(string section, string key, string val, string filePath);
			[DllImport("kernel32", SetLastError = true)]
			public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
		}
		/// <summary>
		/// INIFile Constructor.
		/// </summary>
		/// <param name="INIPath"></param>
		public IniFile(string INIPath)
		{
			path = INIPath;
		}

		/// <summary>
		/// Write Data to the INI File
		/// </summary>
		/// <param name="Section"></param>
		/// Section name
		/// <param name="Key"></param>
		/// Key Name
		/// <param name="Value"></param>
		/// Value Name
		public void IniWriteValue(string Section, string Key, string Value)
		{
			NativeMethods.WritePrivateProfileString(Section, Key, Value, path);
		}

		public void IniWriteValue(string Section, string Key, int Value)
		{
			IniWriteValue(Section, Key, Value.ToString());
		}

		public void IniWriteValue(string Section, string Key, bool Value)
		{
			IniWriteValue(Section, Key, Value.ToString());
		}

		private const int maxSize = 0xFFFF;
		private StringBuilder retVal = new StringBuilder(maxSize);

		/// <summary>
		/// Read Data Value From the Ini File
		/// </summary>
		/// <param name="Section"></param>
		/// <param name="Key"></param>
		/// <returns></returns>
		public string IniReadValue(string Section, string Key)
		{
			int n = NativeMethods.GetPrivateProfileString(Section, Key, "", retVal, maxSize, path);
			if (n >= maxSize - 2)
			{
				int err = Marshal.GetLastWin32Error();
				throw new System.Exception("Слишком большая строка в ini файле");
			}
			return retVal.ToString();
		}

		public string IniReadValue(string Section, int Key)
		{
			return IniReadValue(Section, Key.ToString());
		}

		public int IniReadIntValue(string Section, string Key)
		{
			return Convert.ToInt32(IniReadValue(Section, Key));
		}

		public int IniReadIntValue(string Section, string Key, int defValue)
		{
			int value;
			if (!Int32.TryParse(IniReadValue(Section, Key), out value))
				value = defValue;
			return value;
		}

		public string[] IniReadArray(string Section, string Key)
		{
			return IniReadValue(Section, Key).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public bool IniReadBoolValue(string Section, string Key)
		{
			return Convert.ToBoolean(IniReadValue(Section, Key));
		}

		public bool IniReadBoolValue(string Section, string Key, bool defValue)
		{
			bool value;
			if (!Boolean.TryParse(IniReadValue(Section, Key), out value))
				value = defValue;
			return value;
		}

		public DateTime IniReadDateValue(string Section, string Key)
		{
			return Convert.ToDateTime(IniReadValue(Section, Key));
		}

		public DateTime IniReadDateValue(string Section, string Key, DateTime defValue)
		{
			DateTime value;
			if (!DateTime.TryParse(IniReadValue(Section, Key), out value))
				value = defValue;
			return value;
		}
	}
}
