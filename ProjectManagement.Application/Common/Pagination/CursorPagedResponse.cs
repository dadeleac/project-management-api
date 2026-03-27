namespace ProjectManagement.Application.Common.Pagination
{
    public sealed record CursorPagedResponse<TItem>(
        IReadOnlyCollection<TItem> Items,
        string? NextCursor,
        bool HasMore);
}
