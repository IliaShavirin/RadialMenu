using System;
using System.ComponentModel;
using System.IO;

namespace BaseProj.SharpVectors.SharpVectorRuntime
{
    [Serializable]
    [TypeConverter(typeof(EmbeddedBitmapDataConverter))]
    public struct EmbeddedBitmapData
    {
        #region Private Fields

        private MemoryStream _stream;

        #endregion

        #region Constructors

        public EmbeddedBitmapData(MemoryStream stream)
        {
            _stream = stream;
        }

        #endregion

        #region Public Properties

        public MemoryStream Stream
        {
            get => _stream;
            set => _stream = value;
        }

        #endregion
    }
}