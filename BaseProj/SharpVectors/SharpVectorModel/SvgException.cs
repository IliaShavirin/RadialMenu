using System;
using System.Runtime.Serialization;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;

namespace BaseProj.SharpVectors.SharpVectorModel
{
    [Serializable]
    public class SvgException : DomException
    {
        public SvgException(SvgExceptionType errorCode) : this(errorCode, string.Empty, null)
        {
        }

        public SvgException(SvgExceptionType errorCode, string message) : this(errorCode, message, null)
        {
        }

        public SvgException(SvgExceptionType errorCode, string message, Exception innerException) : base(message,
            innerException)
        {
            Code = errorCode;
        }

        protected SvgException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public new SvgExceptionType Code { get; }
    }
}