using AutoMapper; 
using FluentValidation; 
using MediatR;
using Microsoft.Extensions.DependencyInjection; 
using System.Reflection; 
using ADR_T.ProductCatalog.Application.Common.Behaviors; 
namespace ADR_T.ProductCatalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceLoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(Assembly.GetExecutingAssembly()); 
        });

        services.AddSingleton(mapperConfiguration);
        services.AddSingleton<IMapper>(sp => mapperConfiguration.CreateMapper());

        return services;
    }
}