using ProjectManagement.Application.Common.Errors;

namespace ProjectManagement.Application.Common.Exceptions
{
    public sealed class NotFoundException : Exception
    {
        public string EntityName { get; }
        public object Key { get; }
        public string Code { get; }

        public NotFoundException(string entityName, object key) 
            : base(ApplicationErrors.NotFoundSummary)
        {
            EntityName = entityName;
            Key = key;
            Code = ApplicationErrors.NotFoundCode;
        }

    }
}
