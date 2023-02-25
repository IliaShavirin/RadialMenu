using System;
using System.Net;

namespace BaseProj.SharpVectors.SharpVectorCore.Utils.Net
{
    [Serializable]
    public sealed class DataWebRequest : WebRequest, IWebRequestCreate
    {
        //only for use from Register();
        private DataWebRequest()
        {
        }

        public DataWebRequest(Uri uri)
        {
            RequestUri = uri;
        }

        public override Uri RequestUri { get; }

        public new WebRequest Create(Uri uri)
        {
            return new DataWebRequest(uri);
        }

        public static bool Register()
        {
            return RegisterPrefix("data", new DataWebRequest());
        }

        public override WebResponse GetResponse()
        {
            return new DataWebResponse(RequestUri);
        }
    }
}