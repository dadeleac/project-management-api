using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces
{
    public interface ITaskItemRepository
    {
        Task CreateAsync(TaskItem taskItem, CancellationToken ct);
        Task<bool> HasInProgressTasksAsync(Guid projectId, CancellationToken ct);
        Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct);
        Task UpdateAsync(TaskItem taskItem, CancellationToken ct);
    }
}
