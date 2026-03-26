using FluentValidation;
using ProjectManagement.Domain.Constraints;
using ProjectManagement.Domain.Errors;

namespace ProjectManagement.Application.Projects.Commands.CreateProject
{
    public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
    {
        public CreateProjectCommandValidator() { 
        
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithErrorCode(DomainErrors.Project.NameRequired.Code)
                .WithMessage(DomainErrors.Project.NameRequired.MessageKey)
                .MaximumLength(DomainConstraints.ProjectNameMaxLength)
                .WithErrorCode(DomainErrors.Project.NameTooLong.Code)
                .WithMessage(DomainErrors.Project.NameTooLong.MessageKey);
            
            RuleFor(x => x.OwnerId)
                .NotEmpty()
                .WithErrorCode(DomainErrors.Project.OwnerRequired.Code)
                .WithMessage(DomainErrors.Project.OwnerRequired.MessageKey);
        }
    }
}
