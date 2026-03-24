using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Common.Behaviors;


namespace ProjectManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(configuration => {                 
                configuration.RegisterServicesFromAssembly(assembly);
                configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(assembly);

            return services; 
        }

    }
}
