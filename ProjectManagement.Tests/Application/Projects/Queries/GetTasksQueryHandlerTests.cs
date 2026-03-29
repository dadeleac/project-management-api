using FluentAssertions;
using Moq;
using ProjectManagement.Application.Common.Interfaces.Queries;
using ProjectManagement.Application.Common.Pagination;
using ProjectManagement.Application.TaskItems.Queries.GetTaskItems;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Tests.Application.Tasks.Queries;

public class GetTasksQueryHandlerTests
{
    private readonly Mock<ITaskQueryService> _queryServiceMock;
    private readonly GetTasksQueryHandler _handler;

    public GetTasksQueryHandlerTests()
    {
        _queryServiceMock = new Mock<ITaskQueryService>();
        _handler = new GetTasksQueryHandler(_queryServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedTasks_WhenRequestIsValid()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        // Definimos valores coherentes
        int pageNumber = 1;
        int pageSize = 10;

        var query = new GetTasksQuery(projectId, TaskItemStatus.Todo, null, pageNumber, pageSize);

        var expectedTasks = new List<TaskItemDto>
    {
        new(Guid.NewGuid(), "Tarea 1", "Desc", TaskItemStatus.Todo, TaskPriority.High, null, null)
    };

        var pagedResponse = new PagedResponse<TaskItemDto>(
            items: expectedTasks,
            pageNumber: pageNumber,
            pageSize: pageSize,
            totalCount: expectedTasks.Count);

        _queryServiceMock
            .Setup(x => x.GetPagedAsync(
                projectId,
                query.Status,
                query.Priority,
                query.NormalizedPageNumber,
                query.NormalizedPageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);

        _queryServiceMock.Verify(x => x.GetPagedAsync(
            projectId,
            query.Status,
            query.Priority,
            query.NormalizedPageNumber,
            query.NormalizedPageSize,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}