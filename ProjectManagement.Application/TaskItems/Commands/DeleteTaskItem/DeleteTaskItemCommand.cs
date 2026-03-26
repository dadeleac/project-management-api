using MediatR;

namespace ProjectManagement.Application.TaskItems.Commands.DeleteTaskItem
{
    public sealed record DeleteTaskItemCommand(Guid TaskItemId) : IRequest;
}
