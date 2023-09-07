using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Analytics.Database.Redis; 

public sealed class Connect : IConnect
{
    private readonly ILogger _logger;
    private readonly DbConnector _dbConnector;
    private ConnectionMultiplexer? _connector;
    private readonly IDatabase? _database = null;
    
    private const int MaxRetries = 3;
    private const int RetryDelayMilliseconds = 2000;

    public Connect(ILogger logger, DbConnector dbConnector)
    {
        _logger = logger;
        _dbConnector = dbConnector;
        _database = GetDatabase();
    }

    public string Read(string key)
    {
        return _database?.StringGet(key) ?? string.Empty;
    }

    public List<string> ReadLike(string keyPattern)
    {
        var records = new List<string>();
        var servers = _dbConnector.GetConnection().GetServers();

        foreach (var server in servers)
        {
            var keys = server.Keys(pattern: keyPattern).Select(k => (string)k).ToArray();
            
            foreach (var key in keys)
            {
                var value = Read(key);
                if (!string.IsNullOrEmpty(value))
                {
                    records.Add(value);
                }
            }
        }
        
        return records;
    }
    

    public void Save(string key, string value)
    {
        _database?.StringSet(key, value);
    }
    
    public void Dispose()
    {
        _connector?.Dispose();
    }
    
    private IDatabase GetDatabase()
    {
        var retryCount = 0;
        var lastException = "";
    
        while (retryCount < MaxRetries)
        {
            try
            {
                _connector =  _dbConnector.GetConnection();

                return _connector.GetDatabase();
            }
            catch (Exception ex)
            {
                retryCount++;

                if (retryCount == MaxRetries)
                {
                    lastException = ex.Message;
                }
                
                _logger.LogInformation(
                    $"Connection attempt {retryCount}/{MaxRetries} failed. Retrying in {RetryDelayMilliseconds}ms...");
                Thread.Sleep(RetryDelayMilliseconds);
            }
            finally
            {
                if (! string.IsNullOrEmpty(lastException))
                {
                    _logger.LogInformation("Disposing the connection now!!");
                    _connector?.Dispose();
                }
            }
        }

        _logger.LogError($"Connection attempts exhausted. Last exception: {lastException}");
        throw new Exception("Failed to establish a database connection.");
    }
}