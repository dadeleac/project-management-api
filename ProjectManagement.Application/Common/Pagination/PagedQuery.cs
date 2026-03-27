using MediatR;

namespace ProjectManagement.Application.Common.Pagination
{
    public abstract record PagedQuery<TResponse>(
        int PageNumber = PaginationDefaults.DefaultPageNumber,
        int PageSize = PaginationDefaults.DefaultPageSize)
        : IRequest<PagedResponse<TResponse>>
    {
        public int NormalizedPageNumber => 
            PageNumber <= 0 
                ? PaginationDefaults.DefaultPageNumber 
                : PageNumber;

        public int NormalizedPageSize => 
            PageSize <= 0 
                ? PaginationDefaults.DefaultPageSize 
                : Math.Min(PageSize, PaginationDefaults.MaxPageSize);

        public int Skip => (NormalizedPageNumber - 1) * NormalizedPageSize;
    }
}
