using FluentValidation.TestHelper;
using ProjectManagement.Application.Projects.Commands.CreateProject;
using ProjectManagement.Domain.Constraints;
using ProjectManagement.Domain.Errors;

namespace ProjectManagement.Tests.Application.Projects.Validators;

public class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectCommandValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new CreateProjectCommand("Proyecto OK", "Descripción", Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new CreateProjectCommand(string.Empty, "Desc", Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(DomainErrors.Project.NameRequired.Code);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_Maximum_Length()
    {
        var longName = new string('a', DomainConstraints.ProjectNameMaxLength + 1);
        var command = new CreateProjectCommand(longName, "Desc", Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(DomainErrors.Project.NameTooLong.Code);
    }

    [Fact]
    public void Should_Have_Error_When_OwnerId_Is_Empty()
    {
        var command = new CreateProjectCommand("Proyecto", "Desc", Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.OwnerId)
            .WithErrorCode(DomainErrors.Project.OwnerRequired.Code);
    }
}