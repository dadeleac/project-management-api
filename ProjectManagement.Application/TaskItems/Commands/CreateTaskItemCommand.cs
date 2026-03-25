using MediatR;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Tasks.Commands
{
    public sealed record CreateTaskItemCommand(
        Guid ProjectId,
        string Title,
        string? Description,
        TaskPriority Priority,
        DateTime? DueDate) : IRequest<Guid>;
}
