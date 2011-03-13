using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TorrentPatcher.TorrentLoader
{
    public delegate void FileCallback(int Precent);

    public class TorrentParser
    {
        private List<TFile> _files;
        private string _HashString;
        private long _InfoEnd;
        private long _InfoStart;
        private string _path;
        private FileCallback _rCallback;
        private Dictionary<string, TVal> _root;
        private byte[] _SHAHash;
        private long _Size;
        private BinaryReader _torrent;
        private long _TotalValues;

        public TorrentParser(string Tpath)
        {
            Process(Tpath);
        }

        public TorrentParser(string TPath, FileCallback ReadNotify)
        {
            _rCallback = ReadNotify;
            Process(TPath);
        }

        private List<TFile> GetFiles()
        {
            List<TFile> list = new List<TFile>();
            if (Info.ContainsKey("files"))
            {
                long num = 0L;
                int num2 = 0;
                int num3 = 0;
                foreach (TVal val in (List<TVal>) Info["files"].dObject)
                {
                    Dictionary<string, TVal> dObject = (Dictionary<string, TVal>) val.dObject;
                    num2 = ((int) (num / PieceLength)) + 1;
                    num += (long) dObject["length"].dObject;
                    num3 = (((int) (num / PieceLength)) + 2) - num2;
                    list.Add(new TFile((long) dObject["length"].dObject, (List<TVal>) dObject["path"].dObject, (long) num2, (long) num3));
                }
                _Size = num;
                return list;
            }
            list.Add(new TFile((long) Info["length"].dObject, (string) Info["name"].dObject, 1L, (long) (Pieces.Length / 20)));
            _Size = (long) Info["length"].dObject;
            return list;
        }

        private string GetSHAHash()
        {
            SHA1Managed managed = new SHA1Managed();
            _torrent.BaseStream.Position = _InfoStart;
            byte[] buffer = _torrent.ReadBytes(Convert.ToInt32(_InfoEnd));
            _SHAHash = managed.ComputeHash(buffer);
            return BitConverter.ToString(_SHAHash).Replace("-", "");
        }

        private static string KeyFind(Dictionary<string, TVal> DictToSearch, string Key)
        {
            if (DictToSearch.ContainsKey(Key))
            {
                return DictToSearch[Key].dObject.ToString();
            }
            return "";
        }

        private static long KeyFindint(Dictionary<string, TVal> DictToSearch, string Key)
        {
            if (DictToSearch.ContainsKey(Key))
            {
                return (long) DictToSearch[Key].dObject;
            }
            return 0L;
        }

        private void Process(string TPath)
        {
            if (!File.Exists(TPath))
            {
                throw new Exception("File doesn't exist");
            }
            _TotalValues = 0L;
            _path = TPath;
            _torrent = new BinaryReader(new FileStream(TPath, FileMode.Open, FileAccess.Read, FileShare.Read), System.Text.Encoding.UTF8);
            if (_torrent.ReadChar() != 'd')
            {
                throw new Exception("Torrent File Error");
            }
            _root = ProcessDict();
            _HashString = GetSHAHash();
            _files = GetFiles();
            _torrent.Close();
        }

        private TVal ProcessByte()
        {
            string s = "";
            do
            {
                s = s + char.ConvertFromUtf32(_torrent.Read()).ToString();
            }
            while (char.ConvertFromUtf32(_torrent.PeekChar()) != ":");
            _torrent.Read();
            byte[] buffer = new byte[int.Parse(s)];
            return new TVal(DataType.Byte, _torrent.ReadBytes(int.Parse(s)));
        }

        private Dictionary<string, TVal> ProcessDict()
        {
            Dictionary<string, TVal> dictionary = new Dictionary<string, TVal>();
            while (_torrent.PeekChar() != 0x65)
            {
                string key = ProcessString();
                if (key == "info")
                {
                    _InfoStart = _torrent.BaseStream.Position;
                }
                if (key == "pieces")
                {
                    dictionary.Add(key, ProcessByte());
                }
                else
                {
                    TVal val = ProcessVal();
                    dictionary.Add(key, val);
                }
                if (key == "info")
                {
                    _InfoEnd = _torrent.BaseStream.Position - _InfoStart;
                }
            }
            _torrent.Read();
            return dictionary;
        }

        private long ProcessInt()
        {
            string s = "";
            do
            {
                s = s + char.ConvertFromUtf32(_torrent.Read()).ToString();
            }
            while (char.ConvertFromUtf32(_torrent.PeekChar()) != "e");
            _torrent.Read();
            return long.Parse(s);
        }

        private List<TVal> ProcessList()
        {
            List<TVal> list = new List<TVal>();
            while (char.ConvertFromUtf32(_torrent.PeekChar()) != "e")
            {
                list.Add(ProcessVal());
            }
            _torrent.Read();
            return list;
        }

        private string ProcessString()
        {
            string s = "";
            do
            {
                s = s + char.ConvertFromUtf32(_torrent.Read()).ToString();
            }
            while (char.ConvertFromUtf32(_torrent.PeekChar()) != ":");
            _torrent.Read();
            byte[] bytes = _torrent.ReadBytes(int.Parse(s));
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        private TVal ProcessVal()
        {
            ReadProgress();
            _TotalValues += 1L;
            switch (char.ConvertFromUtf32(_torrent.PeekChar()))
            {
                case "d":
                    _torrent.Read();
                    return new TVal(DataType.Dictionary, ProcessDict());

                case "l":
                    _torrent.Read();
                    return new TVal(DataType.List, ProcessList());

                case "i":
                    _torrent.Read();
                    return new TVal(DataType.Int, ProcessInt());
            }
            return new TVal(DataType.String, ProcessString());
        }

        private void ReadProgress()
        {
            if (_rCallback != null)
            {
                int precent = (int) (100L / (_torrent.BaseStream.Length / _torrent.BaseStream.Position));
                _rCallback(precent);
            }
        }

        public string[] AnnounceList
        {
            get
            {
                if (!_root.ContainsKey("announce-list"))
                {
                    return new string[0];
                }
                List<string> list = new List<string>();
                string item = "";
                foreach (TVal val in (List<TVal>) _root["announce-list"].dObject)
                {
                    foreach (TVal val2 in (List<TVal>) val.dObject)
                    {
                        item = (string) val2.dObject;
                        if (!list.Contains(item))
                        {
                            list.Add(item);
                        }
                    }
                }
                return list.ToArray();
            }
        }

        public string AnnounceURL
        {
            get
            {
                return KeyFind(_root, "announce");
            }
        }

        public byte[] ByteHash
        {
            get
            {
                return _SHAHash;
            }
        }

        public string Comment
        {
            get
            {
                return KeyFind(_root, "comment");
            }
        }

        public string CreatedBy
        {
            get
            {
                return KeyFind(_root, "created by");
            }
        }

        public DateTime CreationDate
        {
            get
            {
                DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return time.AddSeconds((double) KeyFindint(_root, "creation date"));
            }
        }

        public string Encoding
        {
            get
            {
                return KeyFind(Info, "encoding");
            }
        }

        public int FileCount
        {
            get
            {
                return Files.Count;
            }
        }

        public List<TFile> Files
        {
            get
            {
                return _files;
            }
        }

        public Dictionary<string, TVal> Info
        {
            get
            {
                return (Dictionary<string, TVal>) _root["info"].dObject;
            }
        }

        public bool IsPrivate
        {
            get
            {
                string str = KeyFind(Info, "private");
                if (!(str != ""))
                {
                    return false;
                }
                if (!(str == "1"))
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsSingle
        {
            get
            {
                return !Info.ContainsKey("files");
            }
        }

        public string Name
        {
            get
            {
                return KeyFind(Info, "name.utf-8");
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
                return KeyFindint(Info, "piece length");
            }
        }

        public byte[] Pieces
        {
            get
            {
                return (byte[]) Info["pieces"].dObject;
            }
        }

        public Dictionary<string, TVal> Root
        {
            get
            {
                return _root;
            }
        }

        public string SHAHash
        {
            get
            {
                return _HashString;
            }
        }

        public long Size
        {
            get
            {
                return _Size;
            }
        }

        public long TotalValues
        {
            get
            {
                return _TotalValues;
            }
        }
    }
}

