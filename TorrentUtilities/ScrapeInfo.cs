using System;

namespace TorrentUtilities
{
	public class ScrapeInfo
	{
		long _Seeders;
		long _Leechers;
		long _Downloaded;

		public ScrapeInfo(string ScrapeData)
		{
			_Seeders = long.Parse(GetBetween (ScrapeData, "d8:completei", "e"));
			_Downloaded = long.Parse(GetBetween(ScrapeData, "10:downloadedi", "e"));
			_Leechers = long.Parse(GetBetween(ScrapeData, "10:incompletei", "e"));
		}

		public long Seeders { get { return _Seeders; } }
		public long Leechers { get { return _Leechers; } }
		public long Downloaded { get { return _Downloaded; } }

		static string GetBetween(int start, string text, string from, string untill)
		{
			if (from != "")
				start = text.IndexOf(from, start) + from.Length;
			if (untill != "")
				return text.Substring(start, text.IndexOf(untill, start) - start);
			else
				return text.Substring(start);
		}

		static string GetBetween(string text, string from, string untill)
		{
			return GetBetween(0, text, from, untill);
		}
	}
}
