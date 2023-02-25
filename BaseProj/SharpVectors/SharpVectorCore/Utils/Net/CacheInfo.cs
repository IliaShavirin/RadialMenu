using System;

namespace BaseProj.SharpVectors.SharpVectorCore.Utils.Net
{
    public sealed class CacheInfo
    {
        public CacheInfo(DateTime expires, string etag,
            DateTime lastModified, Uri cachedUri, string contentType)
        {
            Expires = expires;
            ETag = etag;
            LastModified = lastModified;
            CachedUri = cachedUri;
            ContentType = contentType;
        }

        public DateTime Expires { get; }

        public Uri CachedUri { get; }

        public DateTime LastModified { get; }

        public string ETag { get; }

        public string ContentType { get; }
    }
}