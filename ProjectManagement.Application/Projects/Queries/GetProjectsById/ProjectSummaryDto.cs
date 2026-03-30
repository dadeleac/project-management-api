namespace ProjectManagement.Application.Projects.Queries.GetProjectsById
{
    public sealed record ProjectSummaryDto(
        int TotalTasks,
        int CompletedTasks,
        int InProgressTasks,
        int PendingTasks);
}
