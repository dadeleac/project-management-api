using ProjectManagement.Application.Common.Pagination;
using ProjectManagement.Application.TaskItems.Queries.GetTaskItems;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Common.Interfaces.Queries
{
    public interface ITaskQueryService
    {
        Task<PagedResponse<TaskItemDto>> GetPagedAsync(
            Guid projectId,
            TaskItemStatus? status,
            TaskPriority? priority,
            int page,
            int pageSize,
            CancellationToken ct);

        Task<TaskItemDto?> GetByIdAsync(Guid id, CancellationToken ct);
    }
}
