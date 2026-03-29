using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Api.Models.Requests
{
    public sealed record UpdateTaskStatusRequest(TaskItemStatus NewStatus);
}
