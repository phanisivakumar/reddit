using FluentValidation;
using FluentValidation.Results;
using System.Collections.Concurrent;

namespace Analytics.PubSub;

public class PubSubQueue<T>
{
    private readonly ConcurrentQueue<T> _messageQueue = new ConcurrentQueue<T>();
    private readonly object _subscribersLock = new object();
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private readonly Func<T, ValidationResult> _messageValidator;
    private Task _messageProcessingTask;
    private readonly List<Action<T>> _subscribers = new List<Action<T>>();

    public PubSubQueue(Func<T, ValidationResult> validator)
    {
        _messageValidator = validator ?? throw new ArgumentNullException(nameof(validator));
        StartMessageProcessingTask();
    }

    public void Publish(T message)
    {
        ValidationResult validationResult = _messageValidator(message);

        if (!validationResult.IsValid)
        {
            string errorMessage = validationResult.Errors[0].ErrorMessage;
            throw new ValidationException(errorMessage);
        }

        _messageQueue.Enqueue(message);
        
        Console.WriteLine("Publish to Queue!!");
        Console.WriteLine(message);
    }

    public void Subscribe(Action<T> subscriber)
    {
        if (subscriber != null)
        {
            lock (_subscribersLock)
            {
                _subscribers.Add(subscriber);
            }
        }
    }

    public void Unsubscribe(Action<T> subscriber)
    {
        if (subscriber != null)
        {
            lock (_subscribersLock)
            {
                _subscribers.Remove(subscriber);
            }
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _messageProcessingTask.Wait(); // Wait for the message processing task to complete
        _cancellationTokenSource.Dispose();
    }

    private void StartMessageProcessingTask()
    {
        _messageProcessingTask = Task.Run(async () =>
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (_messageQueue.TryDequeue(out T message))
                {
                    List<Action<T>> subscribersCopy;

                    lock (_subscribersLock)
                    {
                        subscribersCopy = new List<Action<T>>(_subscribers);
                    }

                    foreach (var subscriber in subscribersCopy)
                    {
                        subscriber(message);
                        Console.WriteLine("Subscriber message arrived !!");
                        Console.WriteLine(message);
                    }
                }

                await Task.Delay(10); // Delay to avoid busy-waiting
            }
        }, _cancellationTokenSource.Token);
    }
}