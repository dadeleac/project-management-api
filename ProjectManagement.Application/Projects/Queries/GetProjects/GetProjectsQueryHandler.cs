using MediatR;
using ProjectManagement.Application.Common.Interfaces.Queries;
using ProjectManagement.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Application.Projects.Queries.GetProjects
{
    public sealed class GetProjectsQueryHandler
        : IRequestHandler<GetProjectsQuery, PagedResponse<ProjectListItemDto>>
    {
        private readonly IProjectQueryService _projectQueryService;

        public GetProjectsQueryHandler(IProjectQueryService projectQueryService)
        {
            _projectQueryService = projectQueryService;
        }

        public Task<PagedResponse<ProjectListItemDto>> Handle(GetProjectsQuery request, CancellationToken ct)
        {
            return _projectQueryService.GetPagedAsync(
                request.Status, 
                request.NormalizedPageNumber, 
                request.NormalizedPageSize, 
                ct);
        }
    }
}
