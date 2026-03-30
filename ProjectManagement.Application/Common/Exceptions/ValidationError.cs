namespace ProjectManagement.Application.Common.Exceptions
{
    public sealed record ValidationError(string Property, string Code, string Message);
}
