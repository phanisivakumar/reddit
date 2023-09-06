using FluentValidation;
using FluentAssertions;

namespace Analytics.PubSub.Tests;

public class Tests
{
    private class MessageValidator : AbstractValidator<string>
    {
        public MessageValidator()
        {
            RuleFor(message => message)
                .NotEmpty()
                .WithMessage("Message cannot be empty.");
        }
    }

    [Test]
    public void PublishAndSubscribe_Success()
    {
        var pubsub = new PubSubQueue<string>(message =>
        {
            var validator = new MessageValidator();
            return validator.Validate(new ValidationContext<string>(message));
        });

        string receivedMessage = null;
        Action<string> subscriber = message => receivedMessage = message;

        pubsub.Subscribe(subscriber);

        string message = "Test Message";
        pubsub.Publish(message);

        // Add a small delay to allow time for message processing
        Task.Delay(100).Wait();

        receivedMessage.Should().Be(message);
    }

    [Test]
    public void MultipleSubscribers_Success()
    {
        var pubsub = new PubSubQueue<string>(message =>
        {
            var validator = new MessageValidator();
            return validator.Validate(new ValidationContext<string>(message));
        });

        string receivedMessage1 = null;
        string receivedMessage2 = null;

        pubsub.Subscribe(message => receivedMessage1 = message);
        pubsub.Subscribe(message => receivedMessage2 = message);

        string message = "Test Message";
        pubsub.Publish(message);

        // Add a small delay to allow time for message processing
        Task.Delay(100).Wait();

        receivedMessage1.Should().Be(message);
        receivedMessage2.Should().Be(message);
    }

    [Test]
    public void Unsubscribe_Success()
    {
        var pubsub = new PubSubQueue<string>(message =>
        {
            var validator = new MessageValidator(); // Specify the type parameter here
            return validator.Validate(new ValidationContext<string>(message));
        });

        string receivedMessage = null;
        Action<string> subscriber = message => receivedMessage = message;

        pubsub.Subscribe(subscriber);
        pubsub.Unsubscribe(subscriber);

        string message = "Test Message";

        Action publishAction = () => pubsub.Publish(message);

        publishAction.Should().NotThrow(); // Ensure no exception is thrown

        // Wait for a short time to allow the background processing task to finish
        Task.Delay(100).Wait();

        receivedMessage.Should().BeNull();
    }

    [Test]
    public void Publish_WithInvalidMessage_Failure()
    {
        var pubsub = new PubSubQueue<string>(message =>
        {
            var validator = new MessageValidator(); // Specify the type parameter here
            return validator.Validate(new ValidationContext<string>(message));
        });

        string invalidMessage = string.Empty;

        Action publishAction = () => pubsub.Publish(invalidMessage);

        publishAction.Should().Throw<ValidationException>()
            .WithMessage("Message cannot be empty.");
    }

    [Test]
    public void Publish_WithValidMessage_Success()
    {
        var pubsub = new PubSubQueue<string>(message =>
        {
            var validator = new MessageValidator(); // Specify the type parameter here
            return validator.Validate(new ValidationContext<string>(message));
        });

        string validMessage = "Valid Message";

        Action publishAction = () => pubsub.Publish(validMessage);

        publishAction.Should().NotThrow(); // Ensure no exception is thrown
    }

    [Test]
    public void Dispose_ShouldStopProcessing()
    {
        var pubsub = new PubSubQueue<string>(message =>
        {
            var validator = new MessageValidator(); // Specify the type parameter here
            return validator.Validate(new ValidationContext<string>(message));
        });

        string receivedMessage = null;

        pubsub.Subscribe(message =>
        {
            receivedMessage = message;
        });

        string message = "Test Message";

        Action publishAction = () => pubsub.Publish(message);

        publishAction.Should().NotThrow(); // Ensure no exception is thrown

        pubsub.Dispose();

        // Wait for a short time to allow the background processing task to stop
        Task.Delay(100).Wait();

        receivedMessage.Should().BeNull();
    }
}