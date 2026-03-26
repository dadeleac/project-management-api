using ProjectManagement.Domain.Constraints;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Errors;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }
        public ProjectStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid OwnerId { get; private set; }

        private Project() { }

        public Project(string name, string? description, Guid ownerId)
        {
            if(string.IsNullOrWhiteSpace(name)) 
                throw new DomainException(DomainErrors.Project.NameRequired, nameof(Name));

            var normalizedName = name.Trim();

            if (normalizedName.Length > DomainConstraints.ProjectNameMaxLength)
                throw new DomainException(DomainErrors.Project.NameTooLong, nameof(Name));

            if(ownerId == Guid.Empty)
                throw new DomainException(DomainErrors.Project.OwnerRequired, nameof(OwnerId));

            Id = Guid.NewGuid();
            Name = normalizedName;
            Description = description?.Trim();
            Status = ProjectStatus.Active;
            CreatedAt = DateTime.UtcNow;
            OwnerId = ownerId;
        }

        public void Archive()
        {            
            Status = ProjectStatus.Archived;
        }
    }
}
