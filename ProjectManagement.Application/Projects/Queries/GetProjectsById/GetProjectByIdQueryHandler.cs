using MediatR;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Common.Interfaces.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Projects.Queries.GetProjectsById
{
    public sealed class GetProjectByIdQueryHandler(IProjectQueryService queryService)
    : IRequestHandler<GetProjectByIdQuery, ProjectDetailDto>
    {
        private readonly IProjectQueryService _queryService = queryService;

        public async Task<ProjectDetailDto> Handle(GetProjectByIdQuery request, CancellationToken ct)
        {
            var project = await _queryService.GetByIdAsync(request.Id, ct);

            if (project is null)
                throw new NotFoundException(nameof(Project), request.Id);

            return project;
        }
    }
}
