using MediatR;
using ProjectManagement.Application.TaskItems.Queries.GetTaskItems;

namespace ProjectManagement.Application.TaskItems.Queries.GetTaskById
{
    public sealed record GetTaskByIdQuery(Guid Id) : IRequest<TaskItemDto>;
}
