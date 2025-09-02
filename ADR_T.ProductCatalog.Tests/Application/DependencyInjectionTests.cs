using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ADR_T.ProductCatalog.Application;
using ADR_T.ProductCatalog.Application.Common.Behaviors;
using System.Linq;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Application
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void AddApplicationServices_ShouldRegisterMediatR()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();

            // Assert
            Assert.Contains(services, s => s.ServiceType == typeof(IMediator));
        }

        [Fact]
        public void AddApplicationServices_ShouldRegisterValidators()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();

            // Assert
            var validatorServices = services.Where(s =>
                s.ServiceType.IsGenericType &&
                s.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>));
            Assert.NotEmpty(validatorServices);
        }

        [Fact]
        public void AddApplicationServices_ShouldRegisterAutoMapper()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();

            // Assert
            Assert.Contains(services, s => s.ServiceType == typeof(IMapper));
            Assert.Contains(services, s => s.ServiceType == typeof(MapperConfiguration));
        }

        [Fact]
        public void AddApplicationServices_ShouldRegisterPipelineBehaviors()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();

            // Assert
            var behaviors = services.Where(s =>
                s.ServiceType.IsGenericType &&
                s.ServiceType.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>) &&
                s.ImplementationType != null).ToList();
            Assert.Contains(behaviors, b =>
                b.ImplementationType.IsGenericType &&
                b.ImplementationType.GetGenericTypeDefinition() == typeof(ValidationPipelineBehavior<,>));
            Assert.Contains(behaviors, b =>
                b.ImplementationType.IsGenericType &&
                b.ImplementationType.GetGenericTypeDefinition() == typeof(PerformanceLoggingBehavior<,>));
        }
    }
}
