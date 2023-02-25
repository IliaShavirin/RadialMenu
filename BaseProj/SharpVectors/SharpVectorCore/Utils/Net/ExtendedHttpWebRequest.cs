using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace BaseProj.SharpVectors.SharpVectorCore.Utils.Net
{
    [Serializable]
    public sealed class ExtendedHttpWebRequest : WebRequest
    {
        #region Constructors

        public ExtendedHttpWebRequest(Uri uri)
        {
            RequestUri = uri;
        }

        #endregion

        #region RequestUri

        public override Uri RequestUri { get; }

        #endregion

        #region Registration

        public static bool Register()
        {
            return RegisterPrefix("http://", new ExtendedHttpWebRequestCreator());
        }

        #endregion

        private WebRequest getRequest(CacheInfo cacheInfo)
        {
            WebRequest request;
            if (cacheInfo != null &&
                cacheInfo.CachedUri != null &&
                cacheInfo.Expires > DateTime.Now)
                request = Create(cacheInfo.CachedUri);
            else
                request = CreateDefault(RequestUri);

            var hRequest = request as HttpWebRequest;
            if (hRequest != null && cacheInfo != null && cacheInfo.CachedUri != null)
            {
                if (cacheInfo.ETag != null) hRequest.Headers["If-None-Match"] = cacheInfo.ETag;
                if (cacheInfo.LastModified != DateTime.MinValue) hRequest.IfModifiedSince = cacheInfo.LastModified;

                hRequest.Headers["Accept-Encoding"] = "deflate, gzip";
            }

            return request;
        }

        private WebResponse getResponse(WebRequest request, CacheInfo cacheInfo)
        {
            WebResponse response = null;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException webEx)
            {
                var hresp2 = webEx.Response as HttpWebResponse;
                if (hresp2 != null)
                    if (hresp2.StatusCode == HttpStatusCode.NotModified)
                    {
                        if (cacheInfo != null && cacheInfo.CachedUri != null)
                            request = Create(cacheInfo.CachedUri);
                        else
                            request = Create(RequestUri);
                        response = request.GetResponse();
                    }
            }

            return response;
        }

        private CacheInfo processResponse(WebResponse response)
        {
            var hResponse = response as HttpWebResponse;
            CacheInfo cacheInfo = null;

            if (hResponse != null)
            {
                DateTime expires;
                if (hResponse.Headers["Expires"] != null)
                    expires = DateTime.Parse(hResponse.Headers["Expires"]);
                else
                    expires = DateTime.MinValue;

                cacheInfo = new CacheInfo(expires, hResponse.Headers["Etag"], hResponse.LastModified, null,
                    hResponse.ContentType);
            }

            return cacheInfo;
        }

        private Stream processResponseStream(WebResponse response)
        {
            var respStream = response.GetResponseStream();

            var contentEnc = response.Headers["Content-Encoding"];
            if (contentEnc != null)
            {
                contentEnc = contentEnc.ToLower();
                if (contentEnc == "gzip")
                    respStream = new GZipStream(respStream, CompressionMode.Decompress);
                else if (contentEnc == "deflate")
                    respStream = new DeflateStream(respStream, CompressionMode.Decompress);
            }
            else if (RequestUri.ToString().EndsWith(".svgz", StringComparison.OrdinalIgnoreCase))
            {
                // TODO: this is an ugly hack for .svgz files. Fix later!
                respStream = new GZipStream(respStream, CompressionMode.Decompress);
            }

            Stream stream = new MemoryStream();
            var count = 0;
            var buffer = new byte[4096];
            while ((count = respStream.Read(buffer, 0, 4096)) > 0)
                stream.Write(buffer, 0, count);

            stream.Position = 0;

            ((IDisposable)respStream).Dispose();

            return stream;
        }

        public override WebResponse GetResponse()
        {
            var cacheInfo = CacheManager.GetCacheInfo(RequestUri);

            var request = getRequest(cacheInfo);
            var response = getResponse(request, cacheInfo);

            if (response == null) return null;

            var stream = processResponseStream(response);

            if (response is HttpWebResponse)
            {
                var respCacheInfo = processResponse(response);

                CacheManager.SetCacheInfo(RequestUri, respCacheInfo, stream);
            }

            return new ExtendedHttpWebResponse(RequestUri, response, stream, cacheInfo);
        }

        #region CacheManager

        private static ICacheManager cacheManager = new NoCacheManager();

        public static ICacheManager CacheManager
        {
            get => cacheManager;
            set => cacheManager = value;
        }

        #endregion
    }
}