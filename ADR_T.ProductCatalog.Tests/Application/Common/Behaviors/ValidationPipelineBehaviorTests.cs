using FluentValidation;
using FluentValidation.Results;
using MediatR;
using ADR_T.ProductCatalog.Application.Common.Behaviors;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Application.Common.Behaviors
{
    public class ValidationPipelineBehaviorTests
    {
        private readonly Mock<IValidator<TestRequest>> _validatorMock;
        private readonly ValidationPipelineBehavior<TestRequest, TestResponse> _behavior;
        private readonly List<IValidator<TestRequest>> _validators;

        public ValidationPipelineBehaviorTests()
        {
            _validatorMock = new Mock<IValidator<TestRequest>>();
            _validators = new List<IValidator<TestRequest>>();
            _behavior = new ValidationPipelineBehavior<TestRequest, TestResponse>(_validators);
        }

        [Fact]
        public async Task Handle_ShouldCallNext_WhenNoValidators()
        {
            // Arrange
            var request = new TestRequest();
            var response = new TestResponse();
            MediatR.RequestHandlerDelegate<TestResponse> next = (CancellationToken ct) => Task.FromResult(response);

            // Act
            var result = await _behavior.Handle(request, next, CancellationToken.None);

            // Assert
            Assert.Equal(response, result);
        }

        [Fact]
        public async Task Handle_ShouldCallNext_WhenValidationSucceeds()
        {
            // Arrange
            var request = new TestRequest();
            var response = new TestResponse();
            _validators.Add(_validatorMock.Object);

            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            MediatR.RequestHandlerDelegate<TestResponse> next = (CancellationToken ct) => Task.FromResult(response);

            // Act
            var result = await _behavior.Handle(request, next, CancellationToken.None);

            // Assert
            Assert.Equal(response, result);
            _validatorMock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var request = new TestRequest();
            _validators.Add(_validatorMock.Object);

            var validationFailure = new ValidationFailure("PropertyName", "Error message");
            var validationResult = new ValidationResult(new[] { validationFailure });
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            MediatR.RequestHandlerDelegate<TestResponse> next = (CancellationToken ct) => Task.FromResult(new TestResponse());

            // Act & Assert
            await Assert.ThrowsAsync<Core.Domain.Exceptions.ValidationException>(() =>
                _behavior.Handle(request, next, CancellationToken.None));

            _validatorMock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        // Clases de prueba internas
        public class TestRequest : IRequest<TestResponse> { }
        public class TestResponse { }
    }
}
