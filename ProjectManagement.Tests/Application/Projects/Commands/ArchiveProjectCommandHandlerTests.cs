using FluentAssertions;
using Moq;
using ProjectManagement.Application.Common.Interfaces.Commands;
using ProjectManagement.Application.Projects.Commands.ArchiveProject;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Errors;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Tests.Application.Projects.Commands;

public class ArchiveProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
    private readonly ArchiveProjectCommandHandler _handler;

    public ArchiveProjectCommandHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _taskItemRepositoryMock = new Mock<ITaskItemRepository>();
        _handler = new ArchiveProjectCommandHandler(_projectRepositoryMock.Object, _taskItemRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldArchiveProject_WhenNoTasksInProgress()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command = new ArchiveProjectCommand(projectId);
        var project = new Project("Proyecto", "Descripción", Guid.NewGuid());

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Simulamos que NO hay tareas en curso (RN-02)
        _taskItemRepositoryMock
            .Setup(x => x.HasInProgressTasksAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        // 1. Verificamos que el estado de la entidad ha cambiado internamente
        project.Status.Should().Be(ProjectStatus.Archived);

        // 2. Comprobamos que se ha llamado a la persistencia una sola vez
        _projectRepositoryMock.Verify(
            x => x.SaveAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowDomainException_WhenProjectHasTasksInProgress()
    {
        // Arrange - RN-02
        var projectId = Guid.NewGuid();
        var command = new ArchiveProjectCommand(projectId);
        var project = new Project("Proyecto", "Descripción del proyecto", Guid.NewGuid());

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Simulamos que el repositorio detecta tareas en curso
        _taskItemRepositoryMock
            .Setup(x => x.HasInProgressTasksAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        // 1. Verificamos que lanza la excepción de negocio específica
        var exception = await act.Should().ThrowAsync<DomainException>();
        exception.Which.Error.Should().Be(DomainErrors.Project.HasInProgressTasks);

        // 2. Garantizamos que el estado del proyecto NO cambió y no se persistió
        project.Status.Should().Be(ProjectStatus.Active);
        _projectRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}