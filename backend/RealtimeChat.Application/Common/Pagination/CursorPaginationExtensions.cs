using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Common.Pagination;

public static class CursorPaginationExtensions
{
    public static async Task<CursorPaginationResult<T>> ToCursorPageAsync<T>(this IQueryable<T> query, CursorPaginationQuery request, CancellationToken ct)
        where T : BaseEntity
    {
        Validate(request);

        var limit = Math.Clamp(request.Limit, 1, 50);

        if (request.Before is long before)
        {
            query = query.Where(x => x.Id < before).OrderByDescending(x => x.Id);
        }
        else if (request.After is long after)
        {
            query = query.Where(x => x.Id > after).OrderBy(x => x.Id);
        }
        else
        {
            query = query.OrderByDescending(x => x.Id);
        }

        var items = await query.Take(limit + 1).ToListAsync(ct);

        var hasMore = items.Count > limit;
        if (hasMore) items.RemoveAt(items.Count - 1);

        if (request.After is null)
        {
            items.Reverse();
        }

        return new CursorPaginationResult<T>
        {
            Items = items,
            HasMore = hasMore,
            BeforeCursor = items.FirstOrDefault()?.Id,
            AfterCursor = items.LastOrDefault()?.Id
        };
    }

    private static void Validate(CursorPaginationQuery request)
    {
        if (request.Before.HasValue && request.After.HasValue)
        {
            throw new BadRequestException("Before and After cannot be used together.");
        }
    }
}