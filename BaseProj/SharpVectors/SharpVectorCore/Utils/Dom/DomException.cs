using System;
using System.Runtime.Serialization;

namespace BaseProj.SharpVectors.SharpVectorCore.Utils.Dom
{
    [Serializable]
    public class DomException : Exception
    {
        /*public DomException() : this(DomExceptionType.SVGSHARP_UNDEFINED_ERROR, "Unknown error")
        {
        }*/

        protected DomException(string msg, Exception innerException) : base(msg, innerException)
        {
        }

        public DomException(DomExceptionType code) : this(code, string.Empty)
        {
        }

        public DomException(DomExceptionType code, string msg) : this(code, msg, null)
        {
        }

        public DomException(DomExceptionType code, string msg, Exception innerException) : base(msg, innerException)
        {
            Code = code;
        }

        protected DomException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DomExceptionType Code { get; }
    }
}