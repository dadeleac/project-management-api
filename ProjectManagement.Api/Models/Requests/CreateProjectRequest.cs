namespace ProjectManagement.Api.Models.Requests
{
    public sealed record CreateProjectRequest(
        string Name,
        string? Description,
        Guid OwnerId);
}
