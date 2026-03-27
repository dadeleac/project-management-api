using FluentAssertions;
using Moq;
using ProjectManagement.Application.Common.Interfaces.Queries;
using ProjectManagement.Application.Common.Pagination;
using ProjectManagement.Application.Projects.Queries.GetProjects;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Tests.Application.Projects.Queries;

public class GetProjectsQueryHandlerTests
{
    private readonly Mock<IProjectQueryService> _queryServiceMock;
    private readonly GetProjectsQueryHandler _handler;

    public GetProjectsQueryHandlerTests()
    {
        _queryServiceMock = new Mock<IProjectQueryService>();
        _handler = new GetProjectsQueryHandler(_queryServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CallQueryService_WithValidatedPagingValues()
    {
        // Arrange
        var query = new GetProjectsQuery(
            Status: ProjectStatus.Active,
            PageNumber: 0,
            PageSize: 999);

        var expectedResponse = new PagedResponse<ProjectListItemDto>(
            new List<ProjectListItemDto>
            {
                new(Guid.NewGuid(), "Proyecto 1", ProjectStatus.Active, DateTime.UtcNow, Guid.NewGuid())
            },
            query.NormalizedPageNumber,
            query.NormalizedPageSize,
            1);

        _queryServiceMock
            .Setup(x => x.GetPagedAsync(
                query.Status,
                query.NormalizedPageNumber,
                query.NormalizedPageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.PageNumber.Should().Be(query.NormalizedPageNumber);
        result.PageSize.Should().Be(query.NormalizedPageSize);
        result.TotalCount.Should().Be(1);

        _queryServiceMock.Verify(x => x.GetPagedAsync(
            query.Status,
            query.NormalizedPageNumber,
            query.NormalizedPageSize,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}