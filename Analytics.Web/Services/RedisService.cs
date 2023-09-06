using Analytics.Database.Redis;

namespace Analytics.Web.Services;

public class RedisService
{
    private readonly ILogger<RedisService> _logger;
    private readonly DbConnector _dbConnector;

    public RedisService(ILogger<RedisService> logger, DbConnector dbConnector)
    {
        _logger = logger;
        _dbConnector = dbConnector;
    }

    public string GetValueFromDatabase(string key)
    {
        var db = new Connect(_logger, _dbConnector);
        var data = db.Read(key);
        _logger.LogInformation(data);
        
        return data;
    }
}