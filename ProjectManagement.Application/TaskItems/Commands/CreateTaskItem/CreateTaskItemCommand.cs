using MediatR;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.TaskItems.Commands.CreateTaskItem
{
    public sealed record CreateTaskItemCommand(
        Guid ProjectId,
        string Title,
        string? Description,
        TaskPriority Priority,
        DateTime? DueDate) : IRequest<Guid>;
}
