using StackExchange.Redis;

namespace Analytics.Database.Redis;

public interface IConnect : IDisposable
{
    string Read(string key);
    void Save(string key, string value);
}