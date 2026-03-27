using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Projects.Queries.GetProjects
{
    public sealed record ProjectListItemDto(
        Guid Id,
        string Name,
        ProjectStatus Status,
        DateTime CreatedAt,
        Guid OwnerId);
}
