using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TorrentUtilities
{
	public delegate void FileCallback(int Precent);

	/// <summary>
	/// Parses a torrent file and returns the root dictionary, the sha hash and the files size
	/// </summary>
	public class TorrentParser : IDisposable
	{
		#region Inner vars
		Dictionary<string, TVal> _root; //The root dictionary
		List<TFile> _files; //The File List
		BinaryReader _torrent; //The torrent file reader
		long _TotalValues;

		string _HashString; //The torrent SHA1 string
		byte[] _SHAHash; //The SHA1 byte array
		long _InfoStart; //The position where the Info dictionary starts
		long _InfoEnd; //The position where the Info dictionary ends

		string _path; //The torrent path
		long _Size; //The overall size of the downloaded file/s

		//Added 6.5.07
		FileCallback _rCallback; //A reference to update the Progress bar
		#endregion

		#region Torrent Parser
		/// <summary>
		/// The class constructor, starts the parsing process
		/// </summary>
		/// <param name="TPath">The torrent file path</param>
		private void Process(string TPath)
		{
			//Check that the file exists
			if (!(File.Exists(TPath)))
			{
				throw new Exception("File doesn't exist");
			}
			_TotalValues = 0;
			_path = TPath;
			//Open the torrent file
			_torrent = new BinaryReader(new FileStream(TPath, FileMode.Open, FileAccess.Read, FileShare.Read), System.Text.Encoding.UTF8);
			//Checks that the first item in  the torrent is dictionary ("d")
			if (_torrent.ReadChar() != 'd')
			{
				throw new Exception("Torrent File Error");
			}
			//Where it all starts, processing the root dictionary
			_root = (Dictionary<string,TVal>)ProcessDict();
			//Calculating the torrent SHA1 hash (the hash of the Info dictionary
			_HashString = GetSHAHash();
			//Added 25.4.07
			//Creating a convenient file list, and calculating the overall size
			_files = GetFiles();
			//Calculating the overall size of the downloaded file/s

			//Closing the file after all is done
			_torrent.Close();
		}

		/// <summary>
		/// Processing the dictionary starting form the file stream position
		/// </summary>
		/// <returns>The processed dictionary</returns>
		TVal ProcessDict()
		{
			//Defining the new dictionary
			Dictionary<string, TVal> TempDict = new Dictionary<string, TVal>();
			TVal val = new TVal(DataType.Dictionary, TempDict);
			//Looping while it's not the end of the dictionary ('e' or in ASCII 101)
			while (_torrent.PeekChar() != 101)
			{
				//Getting the key
				string key = ReadString();
				//Checking if this is the Info dictionary start
				if (key == "info") { _InfoStart = _torrent.BaseStream.Position; }
				//Checking for binary values
				if (key == "pieces")
				{
					//If this is a binary value,
					//call the binary value handler and add it to the dictionary
					TempDict.Add(key, ProcessBytes());
				}
				else
				{
					//if not,Check which value is it and add to the dictionary
					TVal Val = ProcessVal();
					TempDict.Add(key, Val);
				}
				//if this is the end of the info dictionary then this is the _InfoEnd
				if (key == "info") { _InfoEnd = _torrent.BaseStream.Position - _InfoStart; }
			}
			//Getting rid of the dictionary end ("e")
			_torrent.Read();
			//Returning the dict.
			return val;
		}

		/// <summary>
		/// List handler
		/// </summary>
		/// <returns>The parsed List</returns>
		TVal ProcessList()
		{
			//Defining the new list
			List<TVal> TempList = new List<TVal>();
			TVal val = new TVal(DataType.List, TempList);
			//Lopping while it's not the end of the list ("e")
			while (char.ConvertFromUtf32(_torrent.PeekChar()) != "e")
			{
				//Adding the list value
				TempList.Add(ProcessVal());
			}
			//Getting rid of the list end ("e")
			_torrent.Read();
			//Returning the list
			return val;
		}

		/// <summary>
		/// String handler
		/// </summary>
		/// <returns>The parsed string</returns>
		private string ReadString()
		{
			//Changed 25.4.07
			//Reading the string from the file in a byte array form
			byte[] TempBytes = ReadBytes();
			//Converting it to string
			string TempString = System.Text.Encoding.UTF8.GetString(TempBytes);
			//Returning the string
			return TempString;
		}

		/// <summary>
		/// String handler
		/// </summary>
		/// <returns>The parsed string</returns>
		TVal ProcessString()
		{
			//Reading the string from the file in a byte array form
			byte[] TempBytes = ReadBytes();
			Encoding Enc = System.Text.Encoding.GetEncoding(
					"utf-8",
					new EncoderExceptionFallback(),
					new DecoderExceptionFallback());

			string TempString = "";
			//Converting it to string
			try
			{
				TempString = Enc.GetString(TempBytes);
				Enc = System.Text.Encoding.UTF8;
			}
			catch (System.Text.DecoderFallbackException /*ex*/)
			{				
				Enc = System.Text.Encoding.Default;
				TempString = Enc.GetString(TempBytes);
			}
			//Returning the string
			return new TVal(TempString, Enc);
		}

		/// <summary>
		/// Processes the special binary value
		/// </summary>
		/// <returns>The value in a TVal form</returns>
		TVal ProcessBytes()
		{
			byte[] tempBytes = ReadBytes();

			//Returning the TVal
			return new TVal(DataType.Byte, tempBytes);
		}

		private byte[] ReadBytes()
		{
			//Defining the new string length
			StringBuilder tempLen = new StringBuilder();
			//Looping to get the length
			do
			{
				tempLen.Append(char.ConvertFromUtf32(_torrent.Read()).ToString());
			}
			//While it's not the length end (":")
			while (char.ConvertFromUtf32(_torrent.PeekChar()) != ":");
			//Getting read of the length end (":")
			_torrent.Read();

			//The byte array having the length we've just come up with
			byte[] tempBytes = new byte[int.Parse(tempLen.ToString())];
			//Reading the byte array from the torrent file
			tempBytes = _torrent.ReadBytes(int.Parse(tempLen.ToString()));
			return tempBytes;
		}

		/// <summary>
		/// Integers handlers
		/// </summary>
		/// <returns>The integer</returns>
		TVal ProcessInt()
		{
			//The integer at first will be a string
			StringBuilder TempString = new StringBuilder();
			//Loop to get the integer string
			do
			{
				TempString.Append(char.ConvertFromUtf32(_torrent.Read()).ToString());
			}
			//Until "e" comes up
			while (char.ConvertFromUtf32(_torrent.PeekChar()) != "e");
			//Getting rid of the integer end ("e")
			_torrent.Read();
			long value = long.Parse(TempString.ToString());
			return new TVal(value);
		}

		/// <summary>
		/// Checks the value type and calling their handlers
		/// </summary>
		/// <returns>The value in a TVal form</returns>
		TVal ProcessVal()
		{
			ReadProgress();
			_TotalValues++;
			//Check which value type is it
			string t = char.ConvertFromUtf32(_torrent.PeekChar());
			switch (t[0])
			{
				//A dictionary ("d")
				case 'd':
					//Getting rid of the dictionary start ("d")
					_torrent.Read();
					//Calling the dictionary handler and returning the TVal
					return ProcessDict();
				//A list ("l")
				case 'l':
					//Getting rid of the list start ("l")
					_torrent.Read();
					//Calling the list handler and returning the TVal
					return ProcessList();
				//An integer ("i")
				case 'i':
					//Getting rid of the integer start ("l")
					_torrent.Read();
					//Calling the integer handler and returning the TVal
					return ProcessInt();
				//A string (starts with its length)
				default:
					//Calling the string handler and returning the TVal
					return ProcessString();
			}
		}

		/// <summary>
		/// Getting the SHA1 hash
		/// </summary>
		/// <returns>The SHA hash</returns>
		string GetSHAHash()
		{
			//Defining the new hash
			using (SHA1Managed sha1 = new SHA1Managed())
			{
				//The info dictionary byte array
				byte[] infoValueBytes;
				//Setting the file reader position to the info start
				_torrent.BaseStream.Position = _InfoStart;
				//Reading untill InfoEnd
				infoValueBytes = _torrent.ReadBytes(Convert.ToInt32(_InfoEnd));
				//Returns the hash as a string
				_SHAHash = sha1.ComputeHash(infoValueBytes);
				return BitConverter.ToString(_SHAHash).Replace("-", "");
			}
		}

		///// <summary>
		///// Gets the overall size
		///// </summary>
		///// <returns>The overall size</returns>
		//long GetSize()
		//{
		//    //Defining the size variable
		//    long totalsize = 0;
		//    //Loop to get all the files
		//    foreach (TFile file in Files)
		//    {
		//        //Addind the file size to the overall size
		//        totalsize += Convert.ToInt64(file.Length);
		//    }
		//    return totalsize;
		//}

		/// <summary>
		/// Creates a convenient file list
		/// </summary>
		/// <returns>The new file list</returns>
		List<TFile> GetFiles() //Changed 25.4.2007
		{
			List<TFile> tempFiles = new List<TFile>();
			if (Info.ContainsKey("files"))
			{
				long l = 0;
				int ps = 0;
				int pl = 0;
				foreach (TVal tFile in ((List<TVal>)Info["files"]))
				{
					Dictionary<string, TVal> tDict = (Dictionary<string, TVal>)tFile;
					ps = (int)(l / this.PieceLength) + 1;
					l += (long)(tDict["length"]);
					pl = (int)(l / this.PieceLength) + 2 - ps;
					long length = (long)tDict["length"];
					List<TVal> path = (List<TVal>)tDict["path"];
					tempFiles.Add(new TFile(length, path, ps, pl));
				}
				_Size = l;
			}
			else
			{
				long length = (long)Info["length"];
				string name = (string)Info["name"];
				tempFiles.Add(new TFile(length, name, 1, GetPieces().Length / 20));
				_Size = (long)Info["length"];
			}
			return tempFiles;
		}
		#endregion

		#region Properties
		public string Path
		{
			get { return _path; }
		}

		public string Name
		{
			get { return KeyFind(Info, "name"); }
		}

		public string SHAHash
		{
			get { return _HashString; }
		}

		public byte[] GetByteHash()
		{
			return _SHAHash;
		}

		public long Size
		{
			get { return _Size; }
		}


		public string AnnounceURL
		{
			get { return KeyFind(_root, "announce"); }
		}

		public string[] GetAnnounceList()
		{
			if (!(_root.ContainsKey("announce-list"))) { return new string[] { }; }
			List<string> TempList = new List<string>();
			string TempTracker = "";
			foreach (TVal AnList in (List<TVal>)_root["announce-list"])
			{
				foreach (TVal TrackList in (List<TVal>)AnList)
				{
					TempTracker = (string)TrackList;
					if (!(TempList.Contains(TempTracker))) { TempList.Add(TempTracker); }
				}
			}
			return TempList.ToArray();
		}

		public string CreatedBy
		{
			get { return KeyFind(_root, "created by"); }
		}

		public string Comment
		{
			get { return KeyFind(_root, "comment"); }
		}

		public string Encoding
		{
			get { return KeyFind(Info, "encoding"); }
		}

		public int FileCount
		{
			get { return Files.Count; }
		}

		public bool IsSingle
		{
			get { return !(this.Info.ContainsKey("files")); }
		}

		public long PieceLength
		{
			get { return KeyFindint(Info, "piece length"); }
		}

		public byte[] GetPieces()
		{
			return (byte[])Info["pieces"];
		}

		public DateTime CreationDate
		{
			get
			{
				DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
				dateTime = dateTime.AddSeconds(KeyFindint(_root, "creation date"));
				return dateTime;
			}
		}

		public bool IsPrivate
		{
			get
			{
				string exists = KeyFind(Info, "private");
				if (exists != "") { return exists == "1" ? true : false; }
				else { return false; }
			}
		}

		public long TotalValues
		{
			get { return _TotalValues; }
		}

		//public string[] AnnounceList
		//{
		//    get { }
		//}

		static string KeyFind(Dictionary<string, TVal> DictToSearch, string Key)
		{
			if (DictToSearch.ContainsKey(Key))
			{
				return DictToSearch[Key].dObject.ToString();
			}
			else
			{
				return "";
			}
		}

		static long KeyFindint(Dictionary<string, TVal> DictToSearch, string Key)
		{
			if (DictToSearch.ContainsKey(Key))
			{
				return (long)DictToSearch[Key];
			}
			else
			{
				return 0;
			}
		}

		public Dictionary<string, TVal> Root
		{
			get { return _root; }
		}

		public Dictionary<string, TVal> Info
		{
			get { return (Dictionary<string, TVal>)_root["info"]; }
		}

		//Changed 25.4.2007
		public List<TFile> Files
		{
			get { return _files; }
		}
		#endregion

		#region Reading Callback //Added 6.5.07
		public TorrentParser(string Tpath)
		{
			Process(Tpath);
		}

		public TorrentParser(string TPath, FileCallback ReadNotify)
		{
			_rCallback = ReadNotify;
			Process(TPath);
		}


		private void ReadProgress()
		{
			if (_rCallback == null) { return; }
			int Division = (int)(100 * 1 / (_torrent.BaseStream.Length / _torrent.BaseStream.Position));
			_rCallback(Division);
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
