using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TorrentUtilities
{
	/// <summary>
	/// Writes a Dictionary of type <string, TVal> to a .torrent file
	/// </summary>
	public class TorrentWriter : IDisposable
	{
		#region Inner Vars
		StreamWriter _torrent;
		string _path;
		Dictionary<string, TVal> _root;
		#endregion

		#region Constructors
		public TorrentWriter(TVal val)
		{
			Debug.Assert(val.Type == DataType.Dictionary);
			Dictionary<string, TVal> Root = (Dictionary<string, TVal>)val;
			_root = Root;
		}
		#endregion

		#region Writing
		public void Write(string TPath)
		{
			_path = TPath;
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
			}
			_torrent = new StreamWriter(_path, true);
		}

		private void WriteData(long Data)
		{
			_torrent.Write("i" + Data.ToString() + "e");
		}

		private void WriteData(string Data, Encoding Enc = null)
		{
			if (Enc == null)
				Enc = System.Text.Encoding.UTF8;
			WriteData(Enc.GetBytes(Data));
		}

		private void WriteData(TVal Data)
		{
			switch (Data.Type)
			{
				case DataType.Dictionary:
					WriteData((Dictionary<string, TVal>)Data);
					break;
				case DataType.List:
					WriteData((List<TVal>)Data);
					break;
				case DataType.Byte:
					WriteData((byte[])Data);
					break;
				case DataType.Int:
					WriteData((long)Data);
					break;
				default:
					WriteData((string)Data, Data.dEncoding);
					break;
			}
		}
		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _torrent != null)
				_torrent.Close();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
