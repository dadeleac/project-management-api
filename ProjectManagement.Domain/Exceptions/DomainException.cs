namespace ProjectManagement.Domain.Exceptions
{
    public sealed class DomainException : Exception
    {
        public string Code { get; }
        public string? PropertyName { get; }

        public DomainException(string code, string? propertyName = null) : base(code)
        {
            Code = code;
            PropertyName = propertyName;
        }
    }
}
