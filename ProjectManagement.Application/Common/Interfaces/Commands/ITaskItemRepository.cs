using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces.Commands
{
    public interface ITaskItemRepository
    {
        Task AddAsync(TaskItem taskItem, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct); 

        Task<bool> HasInProgressTasksAsync(Guid projectId, CancellationToken ct);
        Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<TaskItem?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct);
    
    }
}
