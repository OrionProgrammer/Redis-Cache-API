namespace Redis_Cache_API.Cache_Infrastructure;

public abstract class CacheKey<T> : ICacheKey
{
    private const string _defaultTemplate = "{0}__{1}";

    public string FormatTemplate { get; }

    public IEnumerable<string> Parameters { get; }

    public CacheKey(IEnumerable<string> parameters) : this(_defaultTemplate, parameters) { }

    public CacheKey(string formatTemplate, IEnumerable<string> parameters)
    {
        FormatTemplate = formatTemplate ?? _defaultTemplate;
        Parameters = parameters;
    }

    public override string ToString()
    {
        var result = string.Format(FormatTemplate, typeof(T).Name);

        if (Parameters != null)
        {
            result = string.Format(result, Parameters);
        }

        return result.ToLowerInvariant();
    }
}
