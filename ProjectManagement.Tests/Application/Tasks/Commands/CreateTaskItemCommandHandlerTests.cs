using FluentAssertions;
using Moq;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Common.Interfaces.Commands;
using ProjectManagement.Application.TaskItems.Commands.CreateTaskItem;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Errors;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Tests.Application.Tasks.Commands
{
    public class CreateTaskItemCommandHandlerTests
    {
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
        private readonly CreateTaskItemCommandHandler _handler;

        public CreateTaskItemCommandHandlerTests()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _taskItemRepositoryMock = new Mock<ITaskItemRepository>();
            _handler = new CreateTaskItemCommandHandler(_projectRepositoryMock.Object, _taskItemRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenProjectIsArchived()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var command = new CreateTaskItemCommand(
                projectId,
                "Nueva tarea",
                "Descripción de la tarea",
                TaskPriority.High,
                DateTime.UtcNow.AddDays(7));

            var archivedProject = new Project("Proyecto", "Descripción", Guid.NewGuid());
            archivedProject.Archive();

            _projectRepositoryMock
                .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(archivedProject);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            var exception = await act.Should().ThrowAsync<DomainException>();

            // 1. Verificamos que el error de dominio y la propiedad afectada sean correctos
            exception.Which.Error.Should().Be(DomainErrors.Project.IsArchived);
            exception.Which.PropertyName.Should().Be(nameof(Project.Status));

            // 2. Garantizamos que NO se haya persistido nada en la base de datos (Protección de datos)
            _taskItemRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var command = new CreateTaskItemCommand(projectId, "T", null, TaskPriority.Low, null);

            _projectRepositoryMock
                .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Project?)null);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            // 1. Se espera una excepción de tipo NotFoundException
            var exception = await act.Should().ThrowAsync<NotFoundException>();

            // 2. Comprobamos que la excepción contenga el nombre de la entidad y el ID buscado
            exception.Which.EntityName.Should().Be(nameof(Project));
            exception.Which.Key.Should().Be(projectId);
        }

        [Fact]
        public async Task Handle_ShouldCreateTaskItem_WhenProjectIsActive()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var command = new CreateTaskItemCommand(
                projectId,
                "Nueva tarea",
                "Descripción",
                TaskPriority.Medium,
                DateTime.UtcNow.AddDays(3));

            var activeProject = new Project("Proyecto", "Descripción", Guid.NewGuid());

            _projectRepositoryMock
                .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(activeProject);

            // Variable para capturar la tarea que se envía al AddAsync
            TaskItem? capturedTask = null;

            _taskItemRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .Callback<TaskItem, CancellationToken>((task, _) => capturedTask = task)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            // 1. Verificamos que se llamó a AddAsync (intención de crear)
            _taskItemRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()),
                Times.Once);

            // 2. Verificamos que se llamó a SaveChangesAsync (confirmación en BD)
            _taskItemRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            // 3. Verificamos los datos de la tarea capturada
            capturedTask.Should().NotBeNull();
            capturedTask!.ProjectId.Should().Be(projectId);
            capturedTask.Title.Should().Be(command.Title);

            // 4. El resultado debe ser el ID de la tarea capturada
            result.Should().Be(capturedTask.Id);
        }
    }
}