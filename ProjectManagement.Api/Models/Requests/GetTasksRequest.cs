using ProjectManagement.Application.Common.Pagination;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Api.Models.Requests
{
    public sealed record GetTasksRequest(
    TaskItemStatus? Status = null,
    TaskPriority? Priority = null,
    int PageNumber = PaginationDefaults.DefaultPageNumber,
    int PageSize = PaginationDefaults.DefaultPageSize);
}

