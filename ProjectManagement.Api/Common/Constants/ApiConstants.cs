namespace ProjectManagement.Api.Common.Constants
{
    public static class ApiConstants
    {
        public static class ConfigurationErrors
        {
            public const string MissingConnectionString = "La cadena de conexión 'DefaultConnection' no está configurada en appsettings.json.";
        }

        public static class ErrorTypes
        {
            public const string Validation = "ValidationException";
            public const string NotFound = "NotFoundException";
            public const string Domain = "DomainException";
            public const string InternalServer = "InternalServerError";
        }

        public static class Messages
        {
            public const string InternalServerError = "Error interno no controlado.";
        }

        public static class LogTemplates
        {
            public const string CriticalError = "Error crítico en API. {Method} {Path}";
            public const string ControlledError = "Error controlado en API. {Status} {Method} {Path} - {Message}";
        }
    }


}
