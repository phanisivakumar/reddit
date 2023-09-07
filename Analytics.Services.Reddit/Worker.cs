using Analytics.Database.Redis;
using Analytics.PubSub;
using Analytics.Services.Reddit.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Analytics.Services.Reddit
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Tokenizer _tokenService;
        private readonly PubSubQueue<string> _pubSubQueue;
        private readonly IConfiguration _configuration;
        private readonly DbConnector _dbConnector;
        private readonly Enrich _enrich;

        public Worker(ILogger<Worker> logger, Tokenizer tokenService, PubSubQueue<string> pubSubQueue, IConfiguration configuration, DbConnector dbConnector)
        {
            _logger = logger;
            _tokenService = tokenService;
            _pubSubQueue = pubSubQueue;
            _configuration = configuration;
            _dbConnector = dbConnector;
            _enrich = new Enrich();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var accessToken = await _tokenService.GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("Access token is null or empty. Exiting.");
                return;
            }

            var rateLimitter = new RateLimiter();
            var rateLimits = await rateLimitter.CheckRateLimits(accessToken);

            if (rateLimits.RateLimitRemaining == 0)
            {
                var resetTime = DateTimeOffset.FromUnixTimeSeconds(rateLimits.RateLimitReset);
                var currentTime = DateTimeOffset.Now;
                var waitTime = resetTime - currentTime;
                Console.WriteLine($"Waiting for {waitTime.TotalSeconds} seconds...");
                await Task.Delay(waitTime, stoppingToken);
            }
            else
            {
                var subreddits = _configuration.GetSection("Subreddits").Get<List<string>>();

                var tasks = new List<Task>();

                foreach (var subreddit in subreddits)
                {
                    tasks.Add(GetTopVotesAsync(subreddit, accessToken));
                }

                tasks.Add(GetSportsNewsAsync("sports", accessToken));
                // Add more tasks as needed

                await Task.WhenAll(tasks);

                await SubProcessor(stoppingToken);
            }
        }

        private async Task GetTopVotesAsync(string subreddit, string accessToken)
        {
            var topVotesResult = await TopVotes.GetTopVotes(subreddit, accessToken);
            if (!string.IsNullOrEmpty(topVotesResult.Item1))
            {
                var jsonObject = new
                {
                    _id = $"{subreddit}:top1",
                    Title = topVotesResult.Item1,
                    Upvotes = topVotesResult.Item2
                };

                string enrichedMessage = JsonConvert.SerializeObject(jsonObject);

                _logger.LogInformation(enrichedMessage);

                _pubSubQueue.Publish(enrichedMessage);
            }
            else
            {
                _logger.LogInformation("No top votes found!");
            }
        }

        private async Task GetSportsNewsAsync(string subreddit, string accessToken)
        {
            await foreach (var messageReceived in SportsNews.GetSportsData(subreddit, accessToken))
            {
                if (!string.IsNullOrEmpty(messageReceived.Item1))
                {
                    var jsonObject = new
                    {
                        _id = $"{subreddit}:{messageReceived.Item1}",
                        Title = messageReceived.Item1,
                        Author = messageReceived.Item2,
                        NumOfVotes = messageReceived.Item3
                    };

                    string enrichedMessage = JsonConvert.SerializeObject(jsonObject);

                    _logger.LogInformation(enrichedMessage);

                    _pubSubQueue.Publish(enrichedMessage);
                }
            }
        }

        private async Task SubProcessor(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SubProcessor Service is starting.");

            _pubSubQueue.Subscribe(message => HandleMessage(message, stoppingToken));

            Console.WriteLine("Sub Processor Subscribed to Service!!");

            await Task.Delay(Timeout.Infinite, stoppingToken);

            _logger.LogInformation("SubProcessor Service is stopping.");
        }

        private async Task HandleMessage(string message, CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation($"Received message: {message}");

                var _id = Enrich.ExtractIdFromJson(message);

                await DbOperation(stoppingToken, _id, message);

                _logger.LogInformation("Message processed and saved in the database.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
            }
        }

        private async Task DbOperation(CancellationToken stoppingToken, string key, string message)
        {
            var db = new Connect(_logger, _dbConnector);

            _logger.LogInformation("Saving to database started at" + DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                db.Save(key, message);

                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}