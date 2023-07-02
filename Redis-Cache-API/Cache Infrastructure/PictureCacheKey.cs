using Redis_Cache_API.Data;

namespace Redis_Cache_API.Cache_Infrastructure;

public class PictureCacheKey : CacheKey<Picture>
{
    public PictureCacheKey() : this(null!) { }
    public PictureCacheKey(IEnumerable<string> parameters) : base(parameters) { }
}