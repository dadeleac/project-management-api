using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Api.Models.Requests
{
    public sealed record CreateTaskRequest(
        string Title,
        string? Description,
        TaskPriority Priority,
        DateTime? DueDate);
}
