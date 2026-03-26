using MediatR;
using FluentValidation;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Common.Errors;

namespace ProjectManagement.Application.Common.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (!_validators.Any()) 
                return await next();
            
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validators
                .Select(v => v.ValidateAsync(context, ct)));
                       
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if(failures.Count != 0)
            {
                var errors = failures
                    .Select(f => new ValidationError(
                        f.PropertyName ?? string.Empty,
                        string.IsNullOrWhiteSpace(f.ErrorCode) 
                            ? ApplicationErrors.ValidationFailed 
                            : f.ErrorCode, 
                        f.ErrorMessage))
                    .Distinct()
                    .ToList();

                throw new RequestValidationException(ApplicationErrors.ValidationFailed, errors);
            }

            return await next(); 
        }
    }
}
