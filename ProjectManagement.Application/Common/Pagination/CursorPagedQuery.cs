using MediatR;
using ProjectManagement.Application.Common.Pagination;

namespace ProjectManagement.Application.Common.Queries
{
    public abstract record CursorPagedQuery<TItem>(
        string? Cursor = null,
        int Take = 10) : IRequest<CursorPagedResponse<TItem>>;
}
