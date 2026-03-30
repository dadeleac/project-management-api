using MediatR;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Common.Interfaces.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.TaskItems.Queries.GetTaskById
{
    public sealed class GetTaskByIdQueryHandler(ITaskQueryService queryService)
        : IRequestHandler<GetTaskByIdQuery, TaskItemDto>
    {
        private readonly ITaskQueryService _queryService = queryService;

        public async Task<TaskItemDto> Handle(GetTaskByIdQuery request, CancellationToken ct)
        {
            var task = await _queryService.GetByIdAsync(request.Id, ct);

            if (task is null)
            {
                throw new NotFoundException(nameof(TaskItem), request.Id);
            }

            return task;
        }
    }
}
