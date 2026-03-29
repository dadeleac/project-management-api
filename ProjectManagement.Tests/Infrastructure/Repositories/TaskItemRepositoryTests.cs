using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Infrastructure.Common.Persistence;
using ProjectManagement.Infrastructure.Repositories.Commands;

namespace ProjectManagement.Tests.Infrastructure.Repositories;

public class TaskItemRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _context;
    private readonly TaskItemRepository _repository;

    public TaskItemRepositoryTests()
    {
        // Setup de SQLite In-Memory
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new TaskItemRepository(_context);
    }

    [Fact]
    public async Task GetByIdIncludingDeletedAsync_Should_ReturnTask_EvenIfIsDeleted()
    {
        // Arrange
        var project = new Project("Proyecto Test", "Desc", Guid.NewGuid());
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var task = new TaskItem(project.Id, "Tarea a borrar", null, TaskPriority.Low, null);
        task.MarkAsDeleted();

        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdIncludingDeletedAsync(task.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task SaveChangesAsync_Should_PersistStateChanges()
    {
        // Arrange: CREAR PROYECTO PRIMERO
        var project = new Project("Proyecto Test", "Desc", Guid.NewGuid());
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var task = new TaskItem(project.Id, "Tarea Original", null, TaskPriority.Medium, null);
        await _repository.AddAsync(task, CancellationToken.None);
        await _repository.SaveChangesAsync(CancellationToken.None);

        // Act
        var taskFromDb = await _context.TaskItems.FindAsync(task.Id);
        taskFromDb!.UpdateStatus(TaskItemStatus.Done);
        await _repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        _context.Entry(taskFromDb).State = EntityState.Detached;
        var updatedTask = await _context.TaskItems.FindAsync(task.Id);
        updatedTask!.Status.Should().Be(TaskItemStatus.Done);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}