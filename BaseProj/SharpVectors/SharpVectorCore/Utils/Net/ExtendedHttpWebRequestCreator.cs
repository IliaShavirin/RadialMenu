using System;
using System.Net;

namespace BaseProj.SharpVectors.SharpVectorCore.Utils.Net
{
    public sealed class ExtendedHttpWebRequestCreator : IWebRequestCreate
    {
        public WebRequest Create(Uri uri)
        {
            return new ExtendedHttpWebRequest(uri);
        }
    }
}