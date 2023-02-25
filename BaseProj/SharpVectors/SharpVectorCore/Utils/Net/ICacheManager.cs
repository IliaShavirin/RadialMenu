using System;
using System.IO;

namespace BaseProj.SharpVectors.SharpVectorCore.Utils.Net
{
    public interface ICacheManager
    {
        CacheInfo GetCacheInfo(Uri uri);
        void SetCacheInfo(Uri uri, CacheInfo cacheInfo, Stream stream);
    }
}