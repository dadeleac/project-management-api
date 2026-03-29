using MediatR;

namespace ProjectManagement.Application.Projects.Queries.GetProjectsById;

public sealed record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDetailDto>;
