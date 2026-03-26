using FluentValidation.TestHelper;
using ProjectManagement.Application.TaskItems.Commands.CreateTaskItem;
using ProjectManagement.Domain.Constraints;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Errors;

namespace ProjectManagement.Tests.Application.Tasks.Validators;

public class CreateTaskItemCommandValidatorTests
{
    private readonly CreateTaskItemCommandValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new CreateTaskItemCommand(
            Guid.NewGuid(),
            "Tarea válida",
            "Descripción",
            TaskPriority.Medium,
            DateTime.UtcNow.AddDays(2));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_ProjectId_Is_Empty()
    {
        var command = new CreateTaskItemCommand(
            Guid.Empty,
            "Tarea",
            null,
            TaskPriority.Low,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var command = new CreateTaskItemCommand(
            Guid.NewGuid(),
            string.Empty,
            null,
            TaskPriority.Low,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorCode(DomainErrors.TaskItem.TitleRequired.Code);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
    {
        var longTitle = new string('a', DomainConstraints.TaskTitleMaxLength + 1);

        var command = new CreateTaskItemCommand(
            Guid.NewGuid(),
            longTitle,
            null,
            TaskPriority.Low,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorCode(DomainErrors.TaskItem.TitleTooLong.Code);
    }

    [Fact]
    public void Should_Have_Error_When_Priority_Is_Invalid()
    {
        var invalidPriority = (TaskPriority)999;

        var command = new CreateTaskItemCommand(
            Guid.NewGuid(),
            "Tarea",
            null,
            invalidPriority,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Priority)
            .WithErrorCode(DomainErrors.TaskItem.InvalidPriority.Code);
    }

    [Fact]
    public void Should_Have_Error_When_DueDate_Is_In_The_Past()
    {
        var command = new CreateTaskItemCommand(
            Guid.NewGuid(),
            "Tarea",
            null,
            TaskPriority.Low,
            DateTime.UtcNow.AddDays(-1));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DueDate)
            .WithErrorCode(DomainErrors.TaskItem.DueDateInPast.Code);
    }
}