using FluentValidation;
using ProjectManagement.Domain.Errors;

namespace ProjectManagement.Application.Projects.Commands.ArchiveProject
{
    public sealed class ArchiveProjectCommandValidator : AbstractValidator<ArchiveProjectCommand>
    {
        public ArchiveProjectCommandValidator() 
        { 
            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .WithErrorCode(DomainErrors.Project.IdRequired.Code)
                .WithMessage(DomainErrors.Project.IdRequired.MessageKey);
        }
    }
}
