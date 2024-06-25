using System.Collections.Concurrent;

namespace Demo.Worker.ApplicationBuilder;

public interface IDataService
{
    public string? Get(string id);
    public void Set(string id, string value);
}

internal sealed class DataService : IDataService
{
    private static readonly ConcurrentDictionary<string, string> _data = [];

    public string? Get(string id)
    {
        if (_data.TryGetValue(id, out var value))
        {
            return value;
        }

        return default;
    }

    public void Set(string id, string value)
    {
        _data[id] = value;
    }
}
