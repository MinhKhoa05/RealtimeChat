namespace RealtimeChat.Application.Common.Pagination;

public class CursorPaginationQuery
{
    public long? Before { get; init; }
    public long? After { get; init; }
    public int Limit { get; init; } = 30;
}