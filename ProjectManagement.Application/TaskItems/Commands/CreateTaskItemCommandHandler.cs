using MediatR;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Errors;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Application.Tasks.Commands
{
    public class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, Guid>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskItemRepository _taskRepository;

        public CreateTaskItemCommandHandler(IProjectRepository projectRepository, ITaskItemRepository taskRepository)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
        }

        public async Task<Guid> Handle(CreateTaskItemCommand request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var project = await _projectRepository.GetByIdAsync(request.ProjectId, ct);

            if (project is null)
            {
                throw new NotFoundException(nameof(Project), request.ProjectId);
            }

            if (project.Status == ProjectStatus.Archived)
            {
                throw new DomainException(DomainErrors.Project.IsArchived, nameof(Project.Status));
            }

            var taskItem = new TaskItem(
                request.ProjectId,
                request.Title,
                request.Description,
                request.Priority,
                request.DueDate);

            await _taskRepository.CreateAsync(taskItem, ct);
            
            return taskItem.Id;
        }
    }
}
