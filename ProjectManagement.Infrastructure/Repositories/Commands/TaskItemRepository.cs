using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces.Commands;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Infrastructure.Common.Persistence;

namespace ProjectManagement.Infrastructure.Repositories.Commands;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly ApplicationDbContext _context;

    public TaskItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.TaskItems.FindAsync(new object[] { id }, ct);
    }

    public async Task<bool> HasInProgressTasksAsync(Guid projectId, CancellationToken ct)
    {
        return await _context.TaskItems
            .AnyAsync(t => t.ProjectId == projectId && t.Status == TaskItemStatus.InProgress, ct);
    }

    public async Task<TaskItem?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct)
    {
        return await _context.TaskItems
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id, ct); 
    }

    public async Task AddAsync(TaskItem taskItem, CancellationToken ct)
    {
        await _context.TaskItems.AddAsync(taskItem, ct); ;
    }

    public Task UpdateAsync(TaskItem taskItem, CancellationToken ct)
    {
        _context.TaskItems.Update(taskItem);
        return Task.CompletedTask; 
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}