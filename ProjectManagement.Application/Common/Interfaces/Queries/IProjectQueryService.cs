using ProjectManagement.Application.Common.Pagination;
using ProjectManagement.Application.Projects.Queries.GetProjects;
using ProjectManagement.Application.Projects.Queries.GetProjectsById;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Common.Interfaces.Queries
{
    public interface IProjectQueryService
    {
        Task<ProjectDetailDto?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<PagedResponse<ProjectListItemDto>> GetPagedAsync(
            ProjectStatus? status,
            int page,
            int pageSize,
            CancellationToken ct);
    }
}
