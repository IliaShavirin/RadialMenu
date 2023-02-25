using System;
using System.Runtime.Serialization;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;

namespace BaseProj.SharpVectors.SharpVectorModel
{
    [Serializable]
    public class SvgExternalResourcesRequiredException : DomException
    {
        public SvgExternalResourcesRequiredException()
            : base("", null)
        {
        }

        protected SvgExternalResourcesRequiredException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}