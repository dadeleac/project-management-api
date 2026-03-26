using FluentValidation;
using ProjectManagement.Domain.Constraints;
using ProjectManagement.Domain.Errors;

namespace ProjectManagement.Application.TaskItems.Commands.CreateTaskItem
{
    public class CreateTaskItemCommandValidator : AbstractValidator<CreateTaskItemCommand>
    {
        public CreateTaskItemCommandValidator() 
        {
            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .WithErrorCode(DomainErrors.Project.IdRequired.Code)
                .WithMessage(DomainErrors.Project.IdRequired.MessageKey);

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithErrorCode(DomainErrors.TaskItem.TitleRequired.Code)
                .WithMessage(DomainErrors.TaskItem.TitleRequired.MessageKey)
                .MaximumLength(DomainConstraints.TaskTitleMaxLength)
                .WithErrorCode(DomainErrors.TaskItem.TitleTooLong.Code)
                .WithMessage(DomainErrors.TaskItem.TitleTooLong.MessageKey);

            RuleFor(x => x.Priority)
            .IsInEnum()
            .WithErrorCode(DomainErrors.TaskItem.InvalidPriority.Code)
            .WithMessage(DomainErrors.TaskItem.InvalidPriority.MessageKey);

            RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.DueDate.HasValue)
            .WithErrorCode(DomainErrors.TaskItem.DueDateInPast.Code)
            .WithMessage(DomainErrors.TaskItem.DueDateInPast.MessageKey);
        } 
    }
}
