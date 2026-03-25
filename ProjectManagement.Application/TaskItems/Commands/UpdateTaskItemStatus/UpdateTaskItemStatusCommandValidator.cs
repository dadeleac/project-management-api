using FluentValidation;
using ProjectManagement.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Application.TaskItems.Commands.UpdateTaskItemStatus
{
    public sealed class UpdateTaskItemStatusCommandValidator : AbstractValidator<UpdateTaskItemStatusCommand>
    {
        public UpdateTaskItemStatusCommandValidator()
        {
            RuleFor(x => x.TaskItemId)
                .NotEmpty()
                .WithErrorCode(DomainErrors.TaskItem.IdRequired.Code)
                .WithMessage(DomainErrors.TaskItem.IdRequired.MessageKey);
            
            RuleFor(x => x.NewStatus)
                .IsInEnum()
                .WithErrorCode(DomainErrors.TaskItem.InvalidStatus.Code)
                .WithMessage(DomainErrors.TaskItem.InvalidStatus.MessageKey);                
        }
    }
}
