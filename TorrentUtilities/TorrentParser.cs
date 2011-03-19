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
    public class TorrentParser
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
            _root = ProcessDict();
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
        Dictionary<string, TVal> ProcessDict()
        {
            //Defining the new dictionary
            Dictionary<string, TVal> TempDict = new Dictionary<string, TVal>();
            //Looping while it's not the end of the dictionary ('e' or in ASCII 101)
            while (_torrent.PeekChar() != 101)
            {
                //Getting the key
                string key = ProcessString();
                //Checking if this is the Info dictionary start
                if (key == "info") { _InfoStart = _torrent.BaseStream.Position; }
                //Checking for binary values
                if (key == "pieces")
                {
                    //If this is a binary value,
                    //call the binary valuse handler and add it to the dictionry
                    TempDict.Add(key, ProcessByte());
                }
                else
                {
                    //if not,Check which value is it and add to the dictionary
                    TVal Val = ProcessVal();
                    TempDict.Add(key, Val);
                }
                //if this is the end of the info dictonary then this is the _InfoEnd
                if (key == "info") { _InfoEnd = _torrent.BaseStream.Position - _InfoStart; }
            }
            //Getting rid of the dictionary end ("e")
            _torrent.Read();
            //Returning the dict.
            return TempDict;
        }

        /// <summary>
        /// List handler
        /// </summary>
        /// <returns>The parsed List</returns>
        List<TVal> ProcessList()
        {
            //Defining the new list
            List<TVal> TempList = new List<TVal>();
            //Lopping while it's not the end of the list ("e")
            while (char.ConvertFromUtf32(_torrent.PeekChar()) != "e")
            {
                //Adding the list value
                TempList.Add(ProcessVal());
            }
            //Getting rid of the list end ("e")
            _torrent.Read();
            //Returning the list
            return TempList;
        }

        /// <summary>
        /// String handler
        /// </summary>
        /// <returns>The parsed string</returns>
        string ProcessString()
        {
            //Defining the new string length
            StringBuilder tempLen = new StringBuilder();
            //Loop to get string length 
            do
            {
                tempLen.Append(char.ConvertFromUtf32(_torrent.Read()).ToString());
            }
            //until ":" comes up
            while (char.ConvertFromUtf32(_torrent.PeekChar()) != ":");
            //Getting rid of the length end (":")
            _torrent.Read();

            //Changed 25.4.07
            //Reading the string from the file in a byte array form
            byte[] TempBytes = _torrent.ReadBytes(int.Parse(tempLen.ToString()));
            //Converting it to string
            string TempString = System.Text.Encoding.UTF8.GetString(TempBytes);
            //Returning the string
            return TempString;

        }

        /// <summary>
        /// Processes the special binary value
        /// </summary>
        /// <returns>The value in a TVal form</returns>
        TVal ProcessByte()
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
            //Returning the TVal
            return new TVal(DataType.Byte, tempBytes);
        }

        /// <summary>
        /// Integers handlers
        /// </summary>
        /// <returns>The integer</returns>
        long ProcessInt()
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
            return long.Parse(TempString.ToString());
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
            switch (char.ConvertFromUtf32(_torrent.PeekChar()))
            {
                //A dictionary ("d")
                case "d":
                    //Getting rid of the dictionary start ("d")
                    _torrent.Read();
                    //Calling the dictionary handler and returning the TVal
                    return new TVal(DataType.Dictionary, ProcessDict());
                //A list ("l")
                case "l":
                    //Getting rid of the list start ("l")
                    _torrent.Read();
                    //Calling the list handler and returning the TVal
                    return new TVal(DataType.List, ProcessList());
                //An integer ("i")
                case "i":
                    //Getting rid of the integer start ("l")
                    _torrent.Read();
                    //Calling the integer handler and returning the TVal
                    return new TVal(DataType.Int, ProcessInt());
                //A string (starts with its length)
                default:
                    //Calling the string handler and returning the TVal
                    return new TVal(DataType.String, ProcessString());
            }
        }

        /// <summary>
        /// Getting the SHA1 hash
        /// </summary>
        /// <returns>The SHA hash</returns>
        string GetSHAHash()
        {
            //Defining the new hash
            SHA1Managed sha1 = new SHA1Managed();
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
                foreach (TVal tFile in ((List<TVal>)Info["files"].dObject))
                {
                    Dictionary<string, TVal> tDict = (Dictionary<string, TVal>)tFile.dObject;
                    ps = (int)(l / this.PieceLength) + 1;
                    l += (long)(tDict["length"].dObject);
                    pl = (int)(l / this.PieceLength) + 2 - ps;
                    tempFiles.Add(new TFile((long)tDict["length"].dObject, (List<TVal>)tDict["path"].dObject, ps, pl));
                }
                _Size = l;
            }
            else
            {
                tempFiles.Add(new TFile((long)Info["length"].dObject, (string)Info["name"].dObject, 1, this.Pieces.Length / 20));
                _Size = (long)Info["length"].dObject;

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

        public byte[] ByteHash
        {
            get { return _SHAHash; }
        }

        public long Size
        {
            get { return _Size; }
        }


        public string AnnounceURL
        {
            get { return KeyFind(_root, "announce"); }
        }

        public string[] AnnounceList
        {
            get
            {
                if (!(_root.ContainsKey("announce-list"))) { return new string[] { }; }
                List<string> TempList = new List<string>();
                string TempTracker = "";
                foreach (TVal AnList in (List<TVal>)_root["announce-list"].dObject)
                {
                    foreach (TVal TrackList in (List<TVal>)AnList.dObject)
                    {
                        TempTracker = (string)TrackList.dObject;
                        if (!(TempList.Contains(TempTracker))) { TempList.Add(TempTracker); }
                    }
                }
                return TempList.ToArray();
            }
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

        public byte[] Pieces
        {
            get { return (byte[])Info["pieces"].dObject; }
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
                return (long)DictToSearch[Key].dObject;
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
            get { return (Dictionary<string, TVal>)_root["info"].dObject; }
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
    }
}
