namespace RealtimeChat.Application.Interfaces;

public interface IUnitOfWork
{
    Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken ct);
}