using System;

namespace TorrentPatcher
{
    public static class str
    {
        public static string GetBetween(string text, string from)
        {
            return GetBetween(text, from, "");
        }

        public static string GetBetween(int start, string text, string untill)
        {
            return GetBetween(start, text, "", untill);
        }

        public static string GetBetween(string text, string from, int length)
        {
            return GetBetween(0, text, from, length);
        }

        public static string GetBetween(string text, string from, string untill)
        {
            return GetBetween(0, text, from, untill);
        }

        public static string GetBetween(int start, string text, string from, int length)
        {
            if (from != "")
            {
                start = text.IndexOf(from, start) + from.Length;
            }
            return text.Substring(start, length);
        }

        public static string GetBetween(int start, string text, string from, string untill)
        {
            if (from != "")
            {
                start = text.IndexOf(from, start) + from.Length;
            }
            if (untill != "")
            {
                return text.Substring(start, text.IndexOf(untill, start) - start);
            }
            return text.Substring(start);
        }
    }
}

