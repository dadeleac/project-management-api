using FluentValidation;
using ProjectManagement.Domain.Errors;

namespace ProjectManagement.Application.TaskItems.Commands.DeleteTaskItem
{
    public sealed class DeleteTaskItemCommandValidator : AbstractValidator<DeleteTaskItemCommand>
    {
        public DeleteTaskItemCommandValidator() 
        {
            RuleFor(c => c.TaskItemId)
                .NotEmpty()
                .WithErrorCode(DomainErrors.TaskItem.IdRequired.Code)
                .WithMessage(DomainErrors.TaskItem.IdRequired.MessageKey);
        }
    }
}
