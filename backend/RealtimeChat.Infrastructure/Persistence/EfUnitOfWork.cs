using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Infrastructure.Persistence;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public EfUnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    // Thực thi một luồng nghiệp vụ gồm nhiều bước trong cùng một transaction,
    // kể cả khi một số bước tự gọi SaveChangesAsync().
    public async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            var result = await operation(ct);

            await transaction.CommitAsync(ct);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}