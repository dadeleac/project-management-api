using ProjectManagement.Application.Common.Errors;

namespace ProjectManagement.Application.Common.Exceptions
{
    public sealed class RequestValidationException : Exception
    {
        public string Code { get; }
        public IReadOnlyCollection<ValidationError> Errors { get; }

        public RequestValidationException(string code, IEnumerable<ValidationError> errors)
            : base(ApplicationErrors.ValidationSummary)
        {
            Code = code;
            Errors = (errors ?? Enumerable.Empty<ValidationError>())
                .ToList()
                .AsReadOnly();
        }

    }
}
