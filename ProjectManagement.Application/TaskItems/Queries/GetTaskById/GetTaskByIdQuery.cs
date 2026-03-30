using MediatR;

namespace ProjectManagement.Application.TaskItems.Queries.GetTaskById
{
    public sealed record GetTaskByIdQuery(Guid Id) : IRequest<TaskItemDto>;
}
