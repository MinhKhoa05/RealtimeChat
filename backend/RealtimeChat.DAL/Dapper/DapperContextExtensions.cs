using Dapper;
using Dommel;

namespace RealtimeChat.DAL.Dapper;

public static class DapperContextExtensions
{
    #region CRUD Operations

    public static async Task<T?> GetByIdAsync<T>(this IDapperContext db, object id) where T : class
    {
        await db.EnsureOpenAsync();
        return await db.Connection.GetAsync<T>(id, db.Transaction);
    }

    public static async Task<long> InsertAsync<T>(this IDapperContext db, T entity) where T : class
    {
        await db.EnsureOpenAsync();
        var result = await db.Connection.InsertAsync(entity, db.Transaction);
        return Convert.ToInt64(result);
    }

    public static async Task<bool> UpdateAsync<T>(this IDapperContext db, T entity) where T : class
    {
        await db.EnsureOpenAsync();
        return await db.Connection.UpdateAsync(entity, db.Transaction);
    }

    public static async Task<bool> DeleteAsync<T>(this IDapperContext db, T entity) where T : class
    {
        await db.EnsureOpenAsync();
        return await db.Connection.DeleteAsync(entity, db.Transaction);
    }

    #endregion

    #region Dapper Query Extensions
    public static async Task<List<T>> QueryAsync<T>(this IDapperContext db, string sql, object? param = null)
    {
        await db.EnsureOpenAsync();
        var rows = await db.Connection.QueryAsync<T>(sql, param, db.Transaction);
        return rows.ToList();
    }

    public static async Task<T?> QueryFirstOrDefaultAsync<T>(this IDapperContext db, string sql, object? param = null)
    {
        await db.EnsureOpenAsync();
        return await db.Connection.QueryFirstOrDefaultAsync<T>(sql, param, db.Transaction);
    }

    public static async Task<int> ExecuteAsync(this IDapperContext db, string sql, object? param = null)
    {
        await db.EnsureOpenAsync();
        return await db.Connection.ExecuteAsync(sql, param, db.Transaction);
    }

    public static async Task<T?> ExecuteScalarAsync<T>(this IDapperContext db, string sql, object? param = null)
    {
        await db.EnsureOpenAsync();
        return await db.Connection.ExecuteScalarAsync<T>(sql, param, db.Transaction);
    }

    #endregion
}
