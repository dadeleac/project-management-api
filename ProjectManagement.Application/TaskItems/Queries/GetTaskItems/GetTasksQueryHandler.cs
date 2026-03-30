using MediatR;
using ProjectManagement.Application.Common.Interfaces.Queries;
using ProjectManagement.Application.Common.Pagination;

namespace ProjectManagement.Application.TaskItems.Queries.GetTaskItems
{
    public sealed class GetTasksQueryHandler(ITaskQueryService queryService)
        : IRequestHandler<GetTasksQuery, PagedResponse<TaskItemDto>>
    {
        private readonly ITaskQueryService _queryService = queryService;

        public async Task<PagedResponse<TaskItemDto>> Handle(GetTasksQuery request, CancellationToken ct)
        {
            return await _queryService.GetPagedAsync(
                request.ProjectId,
                request.Status,
                request.Priority,
                request.NormalizedPageNumber,
                request.NormalizedPageSize,
                ct);
        }
    }
}
