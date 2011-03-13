using System.Runtime.InteropServices;
using System.Text;
using System;

namespace TorrentPatcher
{
    public class IniFile
    {
        public string path;

        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        private const int maxSize = 0xFFFF;
        private StringBuilder retVal = new StringBuilder(maxSize);

        public string IniReadValue(string Section, string Key)
        {
            int n = GetPrivateProfileString(Section, Key, "", retVal, maxSize, path);
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

        public string[] IniReadArray(string Section, string Key)
        {
            return IniReadValue(Section, Key).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public bool IniReadBoolValue(string Section, string Key)
        {
            return Convert.ToBoolean(IniReadValue(Section, Key));
        }

        public DateTime IniReadDateValue(string Section, string Key)
        {
            return Convert.ToDateTime(IniReadValue(Section, Key));
        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, path);
        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
    }
}

