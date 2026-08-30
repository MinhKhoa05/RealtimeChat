using System.Data.Common;
namespace RealtimeChat.DAL.Dapper;

public interface IDapperContext
{
    DbConnection Connection { get; }
    DbTransaction? Transaction { get; }

    Task EnsureOpenAsync();
}