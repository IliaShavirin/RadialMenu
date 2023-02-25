using System;
using System.IO;

namespace BaseProj.SharpVectors.SharpVectorCore.Utils.Net
{
    public sealed class NoCacheManager : ICacheManager
    {
        public CacheInfo GetCacheInfo(Uri uri)
        {
            return null;
        }

        public void SetCacheInfo(Uri uri, CacheInfo cacheInfo, Stream stream)
        {
        }
    }
}