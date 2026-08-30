using System.Data;
using System.Data.Common;
using Dapper;
using Dommel;
using MySqlConnector;
using RealtimeChat.DAL.Transactions;

namespace RealtimeChat.DAL.Dapper;

public class DapperContext : IDapperContext, ITransactionManager, IAsyncDisposable
{
    private readonly DbConnection _connection;
    private DbTransaction? _transaction;

    static DapperContext()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        DommelMapper.SetColumnNameResolver(new SnakeCaseResolver());
        DommelMapper.AddSqlBuilder(typeof(MySqlConnection), new MySqlSqlBuilder());
    }

    public DapperContext(DbConnection connection)
    {
        _connection = connection;
    }

    public DbConnection Connection => _connection;
    public DbTransaction? Transaction => _transaction;

    public async Task EnsureOpenAsync()
    {
        if (_connection.State == ConnectionState.Open)
        {
            return;
        }

        await _connection.OpenAsync();
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await ExecuteInTransactionAsync(async () =>
        {
            await action();
            return true;
        });
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        if (_transaction != null)
        {
            return await action();
        }

        await EnsureOpenAsync();

        await using var transaction = await _connection.BeginTransactionAsync();
        _transaction = transaction;

        try
        {
            var result = await action();

            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            _transaction = null;
        }
    }

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }
}

public sealed class SnakeCaseResolver : IColumnNameResolver
{
    public string ResolveColumnName(System.Reflection.PropertyInfo propertyInfo)
    {
        var text = propertyInfo.Name;
        return string.Concat(text.Select((ch, index) => index > 0 && char.IsUpper(ch) ? "_" + ch : ch.ToString())).ToLowerInvariant();
    }
}
