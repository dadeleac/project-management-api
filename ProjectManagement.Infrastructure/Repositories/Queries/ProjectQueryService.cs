using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces.Queries;
using ProjectManagement.Application.Common.Pagination;
using ProjectManagement.Application.Projects.Queries.GetProjects;
using ProjectManagement.Application.Projects.Queries.GetProjectsById;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Infrastructure.Common.Persistence;

namespace ProjectManagement.Infrastructure.Repositories.Queries
{
    public sealed class ProjectQueryService : IProjectQueryService
    {
        private readonly ApplicationDbContext _context;

        public ProjectQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectDetailDto?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Projects
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ProjectDetailDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Status,
                    p.CreatedAt,
                    p.OwnerId,
                    new ProjectSummaryDto(
                        _context.TaskItems.Count(t => t.ProjectId == p.Id),
                        _context.TaskItems.Count(t => t.ProjectId == p.Id && t.Status == TaskItemStatus.Done),
                        _context.TaskItems.Count(t => t.ProjectId == p.Id && t.Status == TaskItemStatus.InProgress),
                        _context.TaskItems.Count(t => t.ProjectId == p.Id && t.Status == TaskItemStatus.Todo)
                    )
                ))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<PagedResponse<ProjectListItemDto>> GetPagedAsync(ProjectStatus? status, int page, int pageSize, CancellationToken ct)
        {
            var query = _context.Projects.AsNoTracking();

            if(status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .ThenBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProjectListItemDto
                (
                    p.Id,
                    p.Name,
                    p.Status,
                    p.CreatedAt,
                    p.OwnerId
                )).ToListAsync(ct);

            return new PagedResponse<ProjectListItemDto>(items, page, pageSize, totalCount);
        }
    }
}
