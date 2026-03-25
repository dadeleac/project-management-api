using MediatR;

namespace ProjectManagement.Application.Projects.Commands.CreateProject
{
    public sealed record CreateProjectCommand(
        string Name, 
        string? Description, 
        Guid OwnerId) : IRequest<Guid>;
}
