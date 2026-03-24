using ProjectManagement.Application.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
