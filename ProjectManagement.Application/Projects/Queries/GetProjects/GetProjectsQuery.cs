using ProjectManagement.Application.Common.Pagination;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Projects.Queries.GetProjects
{
    public sealed record GetProjectsQuery(
        ProjectStatus? Status = null,
        int PageNumber = PaginationDefaults.DefaultPageNumber,
        int PageSize = PaginationDefaults.DefaultPageSize)
        : PagedQuery<ProjectListItemDto>(PageNumber, PageSize);
}
