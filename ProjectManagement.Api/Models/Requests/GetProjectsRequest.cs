using ProjectManagement.Application.Common.Pagination;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Api.Models.Requests
{
    public sealed record GetProjectsRequest(
        ProjectStatus? Status = null,
        int PageNumber = PaginationDefaults.DefaultPageNumber,
        int PageSize = PaginationDefaults.DefaultPageSize);
}
