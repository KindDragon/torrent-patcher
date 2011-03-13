using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TorrentPatcher.TorrentLoader
{
    public class TorrentWriter
    {
        private Encoding _enc;
        private string _path;
        private Dictionary<string, TVal> _root;
        private StreamWriter _torrent;

        public TorrentWriter(string TPath, Dictionary<string, TVal> Root, Encoding enc)
        {
            _path = TPath;
            _root = Root;
            _enc = enc;
            Write();
        }

        private void Write()
        {
            try
            {
                _torrent = new StreamWriter(File.Create(_path), Encoding.ASCII);
            }
            catch
            {
                return;
            }
            WriteData(_root);
            _torrent.Close();
        }

        private void WriteData(Dictionary<string, TVal> Data)
        {
            _torrent.Write('d');
            foreach (string str in Data.Keys)
            {
                bool flag1 = str == "comment";
                WriteData(str);
                WriteData(Data[str]);
            }
            _torrent.Write('e');
        }

        private void WriteData(List<TVal> Data)
        {
            _torrent.Write('l');
            foreach (TVal val in Data)
            {
                WriteData(val);
            }
            _torrent.Write('e');
        }

        private void WriteData(byte[] Data)
        {
            _torrent.Write(Data.Length);
            _torrent.Write(':');
            _torrent.Close();
            FileStream stream = new FileStream(_path, FileMode.Append, FileAccess.Write);
            stream.Write(Data, 0, Data.Length);
            stream.Close();
            _torrent = new StreamWriter(_path, true);
        }

        private void WriteData(long Data)
        {
            _torrent.Write("i" + Data.ToString() + "e");
        }

        private void WriteData(string Data)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            WriteData(encoding.GetBytes(Data));
        }

        private void WriteData(TVal Data)
        {
            switch (Data.Type)
            {
                case DataType.Int:
                    WriteData((long) Data.dObject);
                    return;

                case DataType.List:
                    WriteData((List<TVal>) Data.dObject);
                    return;

                case DataType.Dictionary:
                    WriteData((Dictionary<string, TVal>) Data.dObject);
                    return;

                case DataType.Byte:
                    WriteData((byte[]) Data.dObject);
                    return;
            }
            WriteData((string) Data.dObject);
        }
    }
}

