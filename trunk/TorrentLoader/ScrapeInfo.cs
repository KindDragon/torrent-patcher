using System;

namespace TorrentPatcher.TorrentLoader
{
    class ScrapeInfo
    {
        private long _Downloaded;
        private long _Leechers;
        private long _Seeders;

        public ScrapeInfo(string ScrapeData)
        {
            _Seeders = long.Parse(str.GetBetween(ScrapeData, "d8:completei", "e"));
            _Downloaded = long.Parse(str.GetBetween(ScrapeData, "10:downloadedi", "e"));
            _Leechers = long.Parse(str.GetBetween(ScrapeData, "10:incompletei", "e"));
        }

        public long Downloaded
        {
            get
            {
                return _Downloaded;
            }
        }

        public long Leechers
        {
            get
            {
                return _Leechers;
            }
        }

        public long Seeders
        {
            get
            {
                return _Seeders;
            }
        }
    }
}

