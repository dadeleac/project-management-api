namespace ProjectManagement.Domain.Errors
{
    public static class DomainErrors
    {
        public static class Project
        {
            public static readonly Error NameRequired = new Error("project.name.required", "project.name.required.message");
            public static readonly Error NameTooLong = new Error("project.name.too_long", "project.name.too_long.message");
            public static readonly Error OwnerRequired = new Error("project.owner.required", "project.owner.required.message");
            public static readonly Error IdRequired = new Error("project.id.required", "project.id.required.message");
            public static readonly Error HasInProgressTasks = new Error("project.has_in_progress_tasks", "project.has_in_progress_tasks.message");
            public static readonly Error IsArchived = new Error("project.is_archived", "project.is_archived.message");
        }

        public static class TaskItem {       
            
            public static readonly Error TitleRequired = new Error("task.title.required", "task.title.required.message");
            public static readonly Error TitleTooLong = new Error("task.title.too_long", "task.title.too_long.message");
            public static readonly Error InvalidPriority = new Error("task.priority.invalid", "task.priority.invalid.message");
            public static readonly Error DueDateInPast = new Error("task.due_date_in_past", "task.due_date_in_past.message");           
        }
    }
}
