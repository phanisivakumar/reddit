using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Analytics.Database.Redis;

public sealed class DbConnector
{
    private readonly ILogger _logger;
    
    public DbConnector(ILogger logger)
    {
        _logger = logger;
    }
    
    public ConnectionMultiplexer GetConnection()
    {
        _logger.LogInformation("Connecting to Production Environment..");
        
        string connectionString = "redis-16411.c1.us-east1-2.gce.cloud.redislabs.com:16411,password=XTQ2cYN0fsQg0YQcsXwXfOW8Ec6uRyKi";
        ConnectionMultiplexer muxer = ConnectionMultiplexer.Connect(connectionString);
        
        if (muxer.IsConnected)
        {
            _logger.LogInformation("Connected to Production Database!!");
        }
        
        return muxer;
    }
}