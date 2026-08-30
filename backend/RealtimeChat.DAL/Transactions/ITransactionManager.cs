
namespace RealtimeChat.DAL.Transactions;

public interface ITransactionManager
{
    Task ExecuteInTransactionAsync(Func<Task> action);
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
}