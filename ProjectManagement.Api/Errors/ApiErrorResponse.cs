namespace ProjectManagement.Api.Errors
{
    public sealed record ApiErrorResponse(
        int Status,
        string Error,
        string Message,
        IReadOnlyCollection<object> Details);
}
