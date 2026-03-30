using ProjectManagement.Application.Common.Pagination;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.TaskItems.Queries.GetTaskItems
{
    public sealed record GetTasksQuery(
        Guid ProjectId,
        TaskItemStatus? Status = null,
        TaskPriority? Priority = null,
        int PageNumber = PaginationDefaults.DefaultPageNumber,
        int PageSize = PaginationDefaults.DefaultPageSize)
        : PagedQuery<TaskItemDto>(PageNumber, PageSize);
}
