using System.Collections.Generic;

namespace TorrentPatcher.TorrentLoader
{
    public class TFile
    {
        private long _FirstPiece;
        private long _len;
        private string _name;
        private string _path;
        private long _PieceLength;

        public TFile(long Size, List<TVal> FullPath, long pStart, long pLen)
        {
            _len = Size;
            _FirstPiece = pStart;
            _PieceLength = pLen;
            _path = "";
            foreach (TVal val in FullPath)
            {
                _path = _path + @"\" + ((string) val.dObject);
            }
            _path = _path.Remove(0, 1);
            int length = _path.LastIndexOf(@"\");
            if (length > 0)
            {
                _name = _path.Substring(length + 1);
                _path = _path.Substring(0, length);
            }
            else
            {
                _name = _path;
                _path = "";
            }
        }

        public TFile(long Size, string FullPath, long pStart, long pLen)
        {
            _len = Size;
            _FirstPiece = pStart;
            _PieceLength = pLen;
            int length = FullPath.LastIndexOf(@"\");
            if (length > 0)
            {
                _path = FullPath.Substring(0, length);
                _name = FullPath.Substring(length + 1);
            }
            else
            {
                _path = "";
                _name = FullPath;
            }
        }

        public long FirstPiece
        {
            get
            {
                return _FirstPiece;
            }
        }

        public long Length
        {
            get
            {
                return _len;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public long PieceLength
        {
            get
            {
                return _PieceLength;
            }
        }
    }
}

