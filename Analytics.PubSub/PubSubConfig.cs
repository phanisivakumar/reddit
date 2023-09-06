using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics.PubSub;

public static class PubSubConfig
{
    public static void AddPubSubQueue<T>(this IServiceCollection services)
    {
        // Configure message validation logic (replace with your validation implementation)
        Func<T, ValidationResult> messageValidator = message =>
        {
            // Add your message validation logic here
            // Return ValidationResult based on validation rules
            return new ValidationResult(); // Default behavior (no validation)
        };

        // Register PubSubQueue<T> as a service
        services.AddSingleton(provider => new PubSubQueue<T>(messageValidator));
    }
}
