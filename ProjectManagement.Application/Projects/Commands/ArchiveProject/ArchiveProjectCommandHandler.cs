using MediatR;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Common.Interfaces.Commands;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Errors;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Application.Projects.Commands.ArchiveProject
{
    public sealed class ArchiveProjectCommandHandler : IRequestHandler<ArchiveProjectCommand>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskItemRepository _taskRepository; 
        
        public ArchiveProjectCommandHandler(IProjectRepository projectRepository, 
            ITaskItemRepository taskRepository)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
        }

        public async Task Handle(ArchiveProjectCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            
            var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
            
            if (project is null)
                throw new NotFoundException(nameof(Project), request.ProjectId);

            var hasInProgressTasks = await _taskRepository.HasInProgressTasksAsync(project.Id, cancellationToken);

            if (hasInProgressTasks)
                throw new DomainException(DomainErrors.Project.HasInProgressTasks, nameof(Project));

            project.Archive();
            await _projectRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
