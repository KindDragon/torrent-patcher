using System;

namespace TorrentPatcher.TorrentLoader
{
    public class TVal
    {
        private object _object;
        private DataType _type;

        public TVal(DataType DType, object DObject)
        {
            _type = DType;
            _object = DObject;
        }

        public object dObject
        {
            get
            {
                return _object;
            }
        }

        public DataType Type
        {
            get
            {
                return _type;
            }
        }
    }
}

