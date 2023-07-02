namespace Redis_Cache_API.Cache_Infrastructure;

public interface ICacheService
{
    T GetData<T>(string key);

    bool SetData<T>(string key, T value, DateTimeOffset expirationTime);

    object RemoveData(string key);
}
