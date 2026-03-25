using MediatR;

namespace ProjectManagement.Application.Projects.Commands.ArchiveProject
{
    public sealed record ArchiveProjectCommand(Guid ProjectId) : IRequest;
}
