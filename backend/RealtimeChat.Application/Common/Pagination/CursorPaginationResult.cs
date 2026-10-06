namespace RealtimeChat.Application.Common.Pagination;

public class CursorPaginationResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];

    public bool HasMore { get; init; }

    public long? BeforeCursor { get; init; }
    public long? AfterCursor { get; init; }

    public CursorPaginationResult<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        return new CursorPaginationResult<TResult>
        {
            Items = Items.Select(mapper).ToList(),
            HasMore = HasMore,
            BeforeCursor = BeforeCursor,
            AfterCursor = AfterCursor
        };
    }
}