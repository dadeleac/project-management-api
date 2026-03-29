using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Infrastructure.Common.Persistence;
using ProjectManagement.Infrastructure.Repositories.Queries;

namespace ProjectManagement.Tests.Infrastructure.Persistence.Queries;

public class ProjectQueryServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _context;
    private readonly ProjectQueryService _service;
    
    public ProjectQueryServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();

        _service = new ProjectQueryService(_context);
    }

    [Fact]
    public async Task GetPagedAsync_Should_ReturnFilteredAndPagedProjects_WithCorrectMapping()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        var archivedProject = new Project("Proyecto archivado", "Desc archivado", ownerId);
        archivedProject.Archive();

        var activeProject1 = new Project("Proyecto activo 1", "Desc 1", ownerId);
        var activeProject2 = new Project("Proyecto activo 2", "Desc 2", ownerId);
        var activeProject3 = new Project("Proyecto activo 3", "Desc 3", ownerId);

        _context.Projects.AddRange(
            archivedProject,
            activeProject1,
            activeProject2,
            activeProject3);

        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetPagedAsync(
            ProjectStatus.Active,
            page: 1,
            pageSize: 2,
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(3);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalPages.Should().Be(2);
        result.Items.Should().HaveCount(2);

        result.Items.Should().OnlyContain(x => x.Status == ProjectStatus.Active);
        result.Items.Should().OnlyContain(x => x.OwnerId == ownerId);
        result.Items.Should().OnlyContain(x => !string.IsNullOrWhiteSpace(x.Name));

        var items = result.Items.ToList();

        items[0].Name.Should().Be("Proyecto activo 3");
        items[1].Name.Should().Be("Proyecto activo 2");
    }

    [Fact]
    public async Task GetPagedAsync_Should_ReturnSecondPageCorrectly()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        var project1 = new Project("Proyecto 1", null, ownerId);
        var project2 = new Project("Proyecto 2", null, ownerId);
        var project3 = new Project("Proyecto 3", null, ownerId);

        _context.Projects.AddRange(project1, project2, project3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetPagedAsync(
            ProjectStatus.Active,
            page: 2,
            pageSize: 2,
            CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(3);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(2);
        result.TotalPages.Should().Be(2);
        result.Items.Should().HaveCount(1);
        result.Items.Single().Name.Should().Be("Proyecto 1");
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}