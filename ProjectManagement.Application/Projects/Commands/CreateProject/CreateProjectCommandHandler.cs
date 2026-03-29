using MediatR;
using ProjectManagement.Application.Common.Interfaces.Commands;
using ProjectManagement.Domain.Entities;
namespace ProjectManagement.Application.Projects.Commands.CreateProject
{
    public sealed class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
    {
        private readonly IProjectRepository _projectRepository;

        public CreateProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {

            ArgumentNullException.ThrowIfNull(request);

            var project = new Project(request.Name, request.Description, request.OwnerId);

            await _projectRepository.AddAsync(project, cancellationToken);
            await _projectRepository.SaveChangesAsync(cancellationToken);

            return project.Id; 

        }
    }
}
