namespace Demo.WebApi.Application.Storage;
public interface IStorageQueueClient<T>
{
    Task<string> InsertAsync(T data, string queueName, DateTime? visibilityTimeout = null, DateTime? executionTime = null);

    Task<T> PeekAsync(string queueName);

    Task<T> DequeueAsync(string queueName);
}
