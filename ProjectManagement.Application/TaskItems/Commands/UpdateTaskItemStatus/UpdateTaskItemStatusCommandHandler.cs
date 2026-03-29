using MediatR;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Common.Interfaces.Commands;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Errors;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Application.TaskItems.Commands.UpdateTaskItemStatus
{
    public sealed class UpdateTaskItemStatusCommandHandler : IRequestHandler<UpdateTaskItemStatusCommand>
    {
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly IProjectRepository _projectRepository;

        public UpdateTaskItemStatusCommandHandler(ITaskItemRepository taskItemRepository, IProjectRepository projectRepository)
        {
            _taskItemRepository = taskItemRepository;
            _projectRepository = projectRepository;
        }

        public async Task Handle(UpdateTaskItemStatusCommand request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);

            var taskItem = await _taskItemRepository.GetByIdAsync(request.TaskItemId, ct);

            if (taskItem is null)
                throw new NotFoundException(nameof(TaskItem), request.TaskItemId);

            var project = await _projectRepository.GetByIdAsync(taskItem.ProjectId, ct);
                
            if (project == null) 
                throw new NotFoundException(nameof(Project), taskItem.ProjectId);

            if (project.Status == ProjectStatus.Archived)
                throw new DomainException(DomainErrors.Project.IsArchived);

            taskItem.UpdateStatus(request.NewStatus);
            await _taskItemRepository.SaveChangesAsync(ct);
        }
    }
}
