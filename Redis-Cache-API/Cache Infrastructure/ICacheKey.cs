namespace Redis_Cache_API.Cache_Infrastructure;
public interface ICacheKey
{
    string FormatTemplate { get; }
    IEnumerable<string> Parameters { get; }
}