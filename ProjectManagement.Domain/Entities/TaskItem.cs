using ProjectManagement.Domain.Constraints;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Errors;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; private set; }
        public Guid ProjectId { get; private set; }
        public string Title { get; private set; } = default!;
        public string? Description { get; private set; }
        public TaskPriority Priority { get; private set; }
        public TaskItemStatus Status { get; private set; }
        public DateTime? DueDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private TaskItem() { }

        public TaskItem(Guid projectId, string title, string? description, TaskPriority priority, DateTime? dueDate)
        {

            if(string.IsNullOrWhiteSpace(title))
                throw new DomainException(DomainErrors.TaskItem.TitleRequired, nameof(Title));
            
            var normalizedTitle = title.Trim();


            if (normalizedTitle.Length > DomainConstraints.TaskTitleMaxLength)
                throw new DomainException(DomainErrors.TaskItem.TitleTooLong, nameof(Title));

            Id = Guid.NewGuid();
            ProjectId = projectId;
            Title = normalizedTitle;
            Description = description?.Trim();
            Priority = priority;
            Status = TaskItemStatus.Todo;
            DueDate = dueDate;
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false; 
        }

        public void UpdateStatus(TaskItemStatus newStatus)
        {
            Status = newStatus;
            
            if(newStatus == TaskItemStatus.Done && CompletedAt is null)
            {
                CompletedAt = DateTime.UtcNow;
            }
        }

        public void MarkAsDeleted() => IsDeleted = true;

    }
}
