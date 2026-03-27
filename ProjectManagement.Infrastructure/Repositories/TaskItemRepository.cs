using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Infrastructure.Common.Persistence;

namespace ProjectManagement.Infrastructure.Persistence.Repositories;

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

    public async Task SaveAsync(TaskItem taskItem, CancellationToken ct)
    {
        var entry = _context.Entry(taskItem);

        if (entry.State == EntityState.Detached)
        {
            await _context.TaskItems.AddAsync(taskItem, ct);
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> HasInProgressTasksAsync(Guid projectId, CancellationToken ct)
    {
        // RN-02: El Global Query Filter (!IsDeleted) ya actúa aquí automáticamente.
        // Verificamos si hay alguna tarea que no esté en estado 'Done'.
        return await _context.TaskItems
            .AnyAsync(t => t.ProjectId == projectId && t.Status != TaskItemStatus.Done, ct);
    }
}