
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.TaskItems.Queries.GetTaskItems
{
    public record TaskItemDto(
        Guid Id,
        string Title,
        string? Description,
        TaskItemStatus Status,
        TaskPriority Priority,
        DateTime? DueDate, 
        DateTime? CompletedAt);
}
