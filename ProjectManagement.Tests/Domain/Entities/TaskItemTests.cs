using FluentAssertions;
using ProjectManagement.Domain.Constraints;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Errors;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Tests.Domain.Entities
{
    public class TaskItemTests
    {
        [Fact]
        public void Constructor_Should_Create_TaskItem_With_Valid_Parameters()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var title = "Tarea de prueba";
            var description = "Esta es una tarea de prueba.";
            var priority = TaskPriority.High;
            DateTime? dueDate = DateTime.UtcNow.AddDays(7);

            // Act
            var taskItem = new TaskItem(projectId, title, description, priority, dueDate);

            // Assert
            taskItem.Id.Should().NotBeEmpty();
            taskItem.ProjectId.Should().Be(projectId);
            taskItem.Title.Should().Be(title);
            taskItem.Description.Should().Be(description);
            taskItem.Priority.Should().Be(priority);
            taskItem.Status.Should().Be(TaskItemStatus.Todo);
            taskItem.DueDate.Should().Be(dueDate);
            taskItem.CompletedAt.Should().BeNull();
            taskItem.IsDeleted.Should().BeFalse();
            taskItem.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrowDomainException_WhenTitleIsInvalid(string invalidTitle)
        {
            var projectId = Guid.NewGuid();
            var priority = TaskPriority.Medium;

            Action act = () => new TaskItem(projectId, invalidTitle, null, priority, null);

            act.Should().Throw<DomainException>()
                .Which.Error.Should().Be(DomainErrors.TaskItem.TitleRequired);
        }

        [Fact]
        public void Constructor_ShouldTrimTitleAndDescription()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var title = "  Tarea con espacios  ";
            var description = "  Descripción con espacios  ";
            var priority = TaskPriority.Medium;

            // Act
            var taskItem = new TaskItem(projectId, title, description, priority, null);
            
            // Assert
            taskItem.Title.Should().Be("Tarea con espacios");
            taskItem.Description.Should().Be("Descripción con espacios");
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenTitleIsTooLong()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var title = new string('A', DomainConstraints.TaskTitleMaxLength + 1); 
            var description = "Descripción de prueba";
            var priority = TaskPriority.Low;
            // Act
            Action act = () => new TaskItem(projectId, title, description, priority, null);

            // Assert
            act.Should().Throw<DomainException>()
                .Which.Error.Should().Be(DomainErrors.TaskItem.TitleTooLong);
        }

        [Fact]
        public void UpdateStatus_ShouldSetCompletedAt_WhenStatusIsDone()
        {
            // Arrange
            var task = new TaskItem(Guid.NewGuid(), "Tarea", null, TaskPriority.Medium, null);
            task.CompletedAt.Should().BeNull();

            // Act
            task.UpdateStatus(TaskItemStatus.Done);

            // Assert
            task.Status.Should().Be(TaskItemStatus.Done);
            task.CompletedAt.Should().NotBeNull();
            task.CompletedAt.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void UpdateStatus_ShouldNotChangeCompletedAt_IfAlreadySet()
        {
            var task = new TaskItem(Guid.NewGuid(), "Tarea", null, TaskPriority.Medium, null);
            task.UpdateStatus(TaskItemStatus.Done);
            var firstCompletedAt = task.CompletedAt;

            task.UpdateStatus(TaskItemStatus.Done);

            task.CompletedAt.Should().Be(firstCompletedAt);
        }

        [Fact]
        public void MarkAsDeleted_ShouldSetIsDeletedToTrue()
        {
            // Arrange
            var task = new TaskItem(Guid.NewGuid(), "Tarea", null, TaskPriority.Medium, null);

            // Act
            task.MarkAsDeleted();

            // Assert
            task.IsDeleted.Should().BeTrue();
        }
    }
}
