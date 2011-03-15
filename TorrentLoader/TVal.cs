using System;

namespace TorrentPatcher.TorrentLoader
{   
	/// <summary>
	/// Class to store dictionary values
	/// </summary>
	public class TVal
	{
		DataType _type;
		object _object;

		public TVal(DataType DType, object DObject)
		{
			_type=DType;
			_object=DObject;
		}

		public DataType Type
		{
			get { return _type; }
		}

		public object dObject
		{
			get { return _object; }
		}

        public override string ToString()
        {
            return _object.ToString();
        }
	}
}

