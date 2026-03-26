using MediatR;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.TaskItems.Commands.UpdateTaskItemStatus
{
    public sealed class UpdateTaskItemStatusHandler : IRequestHandler<UpdateTaskItemStatusCommand>
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public UpdateTaskItemStatusHandler(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task Handle(UpdateTaskItemStatusCommand request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);

            var taskItem = await _taskItemRepository.GetByIdAsync(request.TaskItemId, ct);

            if (taskItem is null)
                throw new NotFoundException(nameof(TaskItem), request.TaskItemId); 

            taskItem.UpdateStatus(request.NewStatus);
            await _taskItemRepository.UpdateAsync(taskItem, ct);
        }
    }
}
