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
            _Seeders = long.Parse(str.GetBetween (ScrapeData, "d8:completei", "e"));
            _Downloaded = long.Parse(str.GetBetween(ScrapeData, "10:downloadedi", "e"));
            _Leechers = long.Parse(str.GetBetween(ScrapeData, "10:incompletei", "e"));
        }

        public long Seeders { get { return _Seeders; } }
        public long Leechers { get { return _Leechers; } }
        public long Downloaded { get { return _Downloaded; } }
    }
}
