using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces.Queries;
using ProjectManagement.Application.Common.Pagination;
using ProjectManagement.Application.TaskItems.Queries.GetTaskItems;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Infrastructure.Common.Persistence;

namespace ProjectManagement.Infrastructure.Repositories.Queries
{
    public class TaskQueryService : ITaskQueryService
    {
        private readonly ApplicationDbContext _context;

        public TaskQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskItemDto?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.TaskItems
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new TaskItemDto(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status,
                    t.Priority,
                    t.DueDate))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<PagedResponse<TaskItemDto>> GetPagedAsync(Guid projectId, TaskItemStatus? status, TaskPriority? priority, int page, int pageSize, CancellationToken ct)
        {
            var query = _context.TaskItems
                .AsNoTracking()
                .Where(t => t.ProjectId == projectId);

            if(status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            if (priority.HasValue) 
            {
                query = query.Where(t => t.Priority == priority.Value);
            }

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .ThenBy(t => t.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TaskItemDto(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status,
                    t.Priority,
                    t.DueDate))
                .ToListAsync(ct);

            return new PagedResponse<TaskItemDto>(
                items,
                page,
                pageSize,
                totalCount);
        }
    }
}
