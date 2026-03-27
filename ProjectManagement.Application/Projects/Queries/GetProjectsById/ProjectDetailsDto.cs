using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Projects.Queries.GetProjectsById
{
    public sealed record ProjectDetailsDto(
        Guid Id,
        string Name,
        string? Description,
        ProjectStatus Status,
        DateTime CreatedAt,
        Guid OwnerId,
        ProjectSummaryDto Summary);
}
