using FluentValidation.TestHelper;
using ADR_T.ProductCatalog.Application.Features.Auth.Commands.RegisterUser;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserCommandValidatorTests
    {
        private readonly RegisterUserCommandValidator _validator;

        public RegisterUserCommandValidatorTests()
        {
            _validator = new RegisterUserCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Username_Is_Empty()
        {
            // Arrange
            var command = new RegisterUserCommand("", "test@example.com", "password123");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorMessage("El username es requerido.");
        }

        [Fact]
        public void Should_Have_Error_When_Username_Exceeds_Max_Length()
        {
            // Arrange
            var longUsername = new string('a', 51);
            var command = new RegisterUserCommand(longUsername, "test@example.com", "password123");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorMessage("El username no debe exceder 50 caracteres.");
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            // Arrange
            var command = new RegisterUserCommand("testuser", "", "password123");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("El email es requerido.");
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            // Arrange
            var command = new RegisterUserCommand("testuser", "invalid-email", "password123");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("El email no tiene un formato válido.");
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            // Arrange
            var command = new RegisterUserCommand("testuser", "test@example.com", "");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("El password es requerido.");
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Too_Short()
        {
            // Arrange
            var command = new RegisterUserCommand("testuser", "test@example.com", "12345");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("El password debe tener al menos 6 caracteres.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new RegisterUserCommand("testuser", "test@example.com", "password123");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Username);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }
    }
}
