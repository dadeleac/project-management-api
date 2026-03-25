using FluentAssertions;
using ProjectManagement.Domain.Constraints;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Errors;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Tests.Domain.Entities
{
    public class ProjectTests
    {
        [Fact]
        public void Constructor_ShouldCreateProject_WhenValidParameters()
        {
            // Arrange
            var name = "Proyecto de Prueba";
            var description = "Este es un proyecto de prueba.";
            var ownerId = Guid.NewGuid();

            // Act
            var project = new Project(name, description, ownerId);

            // Assert
            project.Id.Should().NotBeEmpty();
            project.Name.Should().Be(name);
            project.Description.Should().Be(description);
            project.OwnerId.Should().Be(ownerId);
            project.Status.Should().Be(ProjectStatus.Active);
            project.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        public void Constructor_ShouldThrowDomainException_WhenNameIsInvalid(string invalidName)
        {
            // Arrange
            var description = "Este es un proyecto de prueba.";
            var ownerId = Guid.NewGuid();

            // Act
            Action act = () => new Project(invalidName, description, ownerId);

            // Assert
            act.Should().Throw<DomainException>().Which.Error.Should().Be(DomainErrors.Project.NameRequired);
        }


        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenOwnerIdIsEmpty()
        {
            // Arrange
            var name = "Proyecto de Prueba";
            var description = "Este es un proyecto de prueba.";
            var ownerId = Guid.Empty;

            // Act
            Action act = () => new Project(name, description, ownerId);

            // Assert
            act.Should().Throw<DomainException>()
                .Which.Error.Should().Be(DomainErrors.Project.OwnerRequired);
        }

        [Fact]
        public void Constructor_ShouldTrimNameAndDescription()
        {
            // Arrange
            var ownerId = Guid.NewGuid();

            // Act
            var project = new Project("  Proyecto de Prueba  ", "  Descripción  ", ownerId);

            // Assert
            project.Name.Should().Be("Proyecto de Prueba");
            project.Description.Should().Be("Descripción");
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenNameIsTooLong()
        {
            var name = new string('a', DomainConstraints.ProjectNameMaxLength + 1);
            var ownerId = Guid.NewGuid();

            Action act = () => new Project(name, null, ownerId);

            act.Should().Throw<DomainException>()
                .Which.Error.Should().Be(DomainErrors.Project.NameTooLong);
        }

        [Fact]
        public void Archive_ShouldChangeStatusToArchived()
        {
            // Arrange
            var name = "Proyecto de Prueba";
            var description = "Este es un proyecto de prueba.";
            var ownerId = Guid.NewGuid();
            var project = new Project(name, description, ownerId);
            // Act
            project.Archive();
            // Assert
            project.Status.Should().Be(ProjectStatus.Archived);
        }
    }
}
