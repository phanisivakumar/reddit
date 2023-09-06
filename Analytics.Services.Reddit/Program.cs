

using Analytics.Database.Redis;
using Analytics.PubSub;
using Analytics.Services.Reddit;
using Analytics.Services.Reddit.Model;
using Analytics.Services.Reddit.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        // Add custom configuration sources if needed
        config.AddJsonFile("appsettings.json", optional: false);
        //config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        
        services.AddOptions<RedditConfig>()
            .Bind(hostContext.Configuration.GetSection("RedditConfig"));
        
        //services.Configure<RedditConfig>(hostContext.Configuration.GetSection("RedditConfig"));
        services.AddTransient<Tokenizer>();
        services.AddPubSubQueue<string>();
        
        services.AddSingleton<DbConnector>(sp =>
            new DbConnector(sp.GetRequiredService<ILogger<DbConnector>>())
        );
        
        // // Register PubSubQueue as a singleton
        // services.AddSingleton<PubSubQueue<string>>(sp =>
        // {
        //     // Create the message validator if needed (replace with your actual validation logic)
        //     Func<string, ValidationResult> messageValidator = message =>
        //     {
        //         // Replace with your validation logic here
        //         return new ValidationResult(); // Default behavior (no validation)
        //     };
        //
        //     return new PubSubQueue<string>(messageValidator);
        // });

    })
    .Build();

host.Run();