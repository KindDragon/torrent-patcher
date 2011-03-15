using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TorrentPatcher.TorrentLoader
{
    /// <summary>
    /// Writes a Dictionary of type <string, TVal> to a .torrent file
    /// </summary>
    public class TorrentWriter
    {
        #region Inner Vars
        StreamWriter _torrent;
        string _path;
        Dictionary<string, TVal> _root;
        Encoding _enc;
        #endregion

        #region Constructors
        public TorrentWriter(string TPath, Dictionary<string, TVal> Root, Encoding enc)
        {
            _path = TPath;
            _root = Root;
            _enc = enc;
            Write();
        }
        #endregion

        #region Writing
        private void Write()
        {
            try
            {
                //_torrent = new BinaryWriter(new FileStream(_path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite),Encoding.UTF8);
                _torrent = new StreamWriter(File.Create(_path),Encoding.ASCII);
            }
            catch { return; }
            WriteData(_root);
            _torrent.Close();
        }

        private void WriteData(Dictionary<string,TVal> Data)
        {
            _torrent.Write('d');
            foreach (string key in Data.Keys)
            {
                if (key == "comment") { Debug.Print("comment"); }
                WriteData(key);
                WriteData(Data[key]);
            }
            _torrent.Write('e');
        }

        private void WriteData(List<TVal> Data)
        {
            _torrent.Write('l');
            foreach (TVal current in Data)
            {
                WriteData(current);
            }
            _torrent.Write('e');
        }

        private void WriteData(byte[] Data)
        {
            _torrent.Write(Data.Length);
            _torrent.Write(':');
            _torrent.Close();
            using(FileStream test = new FileStream(_path, FileMode.Append, FileAccess.Write))
			{
	            test.Write(Data,0,Data.Length);
	            test.Close();
			}
            _torrent = new StreamWriter(_path, true);
        }

        private void WriteData(long Data)
        {
            _torrent.Write("i" + Data.ToString() + "e");
        }

        private void WriteData(string Data)
        {
            //Data=Encoding.
            //_torrent.Write(Data.Length.ToString() + ":");
            UTF8Encoding enc = new UTF8Encoding();
            WriteData(enc.GetBytes(Data));
        }

        private void WriteData(TVal Data)
        {
            switch (Data.Type)
            {
                case DataType.Dictionary:
                    WriteData((Dictionary<string, TVal>)Data.dObject);
                    break;
                case DataType.List:
                    WriteData((List<TVal>)Data.dObject);
                    break;
                case DataType.Byte:
                    WriteData((byte[])Data.dObject);
                    break;
                case DataType.Int:
                    WriteData((long)Data.dObject);
                    break;
                default:
                    WriteData((string)Data.dObject);
                    break;
            }
        }
        #endregion
    }
}
