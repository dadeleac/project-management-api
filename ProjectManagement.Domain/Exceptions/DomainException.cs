using ProjectManagement.Domain.Errors;

namespace ProjectManagement.Domain.Exceptions
{
    public sealed class DomainException : Exception
    {
        public Error Error { get; }
        public string? PropertyName { get; }

        public DomainException(Error error, string? propertyName = null) : base(error.MessageKey)
        {
            Error = error;
            PropertyName = propertyName;
        }
    }
}
