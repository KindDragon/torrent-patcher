using System.Collections.Generic;

namespace TorrentUtilities
{
    /// <summary>
    /// A Class for the files in the torrent
    /// </summary>
    public class TFile //Added 25.4.2007
    {
        long _len;
        string _path;
        string _name;
        long _FirstPiece;
        long _PieceLength;

        public TFile(long Size, string FullPath,long pStart,long pLen)
        {
            _len = Size;
            _FirstPiece=pStart;
            _PieceLength=pLen;
            int FileNamePos = FullPath.LastIndexOf(@"\");
            if (FileNamePos > 0)
            {
                _path = FullPath.Substring(0, FileNamePos);
                _name = FullPath.Substring(FileNamePos + 1);
            }
            else
            {
                _path = "";
                _name = FullPath;
            }
        }

        public TFile(long Size, List<TVal> FullPath,long pStart,long pLen)
        {
            _len = Size;
            _FirstPiece=pStart;
            _PieceLength=pLen;
            _path="";
            foreach (TVal tPath in FullPath)
            {
                _path += @"\" + (string)tPath;
            }
            _path = _path.Remove(0, 1);
            int FileNamePos = _path.LastIndexOf(@"\");
            if (FileNamePos > 0)
            {
                _name = _path.Substring(FileNamePos + 1);
                _path = _path.Substring(0, FileNamePos);
            }
            else
            {
                _name = _path;
                _path = "";
            }
        }

        public long Length
        {
            get { return _len; }
        }

        public string Path
        {
            get { return _path; }
        }

        public string Name
        {
            get { return _name; }
        }

        public long FirstPiece
        {
        	get {return _FirstPiece;}
        }

        public long PieceLength
        {
        	get {return _PieceLength;}
        }
    }
}

