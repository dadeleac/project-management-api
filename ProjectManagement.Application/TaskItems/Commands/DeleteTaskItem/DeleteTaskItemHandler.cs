using MediatR;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Application.TaskItems.Commands.DeleteTaskItem
{
    public sealed class DeleteTaskItemHandler : IRequestHandler<DeleteTaskItemCommand>
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public DeleteTaskItemHandler(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task Handle(DeleteTaskItemCommand request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);

            var taskItem = await _taskItemRepository.GetByIdAsync(request.TaskItemId, ct);

            if (taskItem is null) 
                throw new NotFoundException(nameof(TaskItem), request.TaskItemId);

            if (taskItem.IsDeleted)
                return; 

            taskItem.MarkAsDeleted();
             await _taskItemRepository.UpdateAsync(taskItem, ct);
        }


    }
}
