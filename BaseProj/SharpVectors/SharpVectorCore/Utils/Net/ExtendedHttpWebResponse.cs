using System;
using System.IO;
using System.Net;

namespace BaseProj.SharpVectors.SharpVectorCore.Utils.Net
{
    [Serializable]
    public sealed class ExtendedHttpWebResponse : WebResponse
    {
        private CacheInfo cacheInfo;
        private WebResponse response;
        private Stream responseStream;
        private Uri responseUri;

        public ExtendedHttpWebResponse(Uri responseUri, WebResponse response, Stream responseStream,
            CacheInfo cacheInfo)
        {
            this.responseUri = responseUri;
            this.response = response;
            this.responseStream = responseStream;
            this.cacheInfo = cacheInfo;
        }

        public override string ContentType
        {
            get
            {
                if (!(response is HttpWebResponse) && cacheInfo != null)
                    return cacheInfo.ContentType;
                return response.ContentType;
            }
        }

        public override Uri ResponseUri
        {
            get
            {
                if (!(response is HttpWebResponse) && cacheInfo != null)
                    return cacheInfo.CachedUri;
                return response.ResponseUri;
            }
        }

        public override Stream GetResponseStream()
        {
            if (responseStream != null && responseStream.CanSeek)
            {
                responseStream.Position = 0;
                return responseStream;
            }

            return response.GetResponseStream();
        }
    }
}