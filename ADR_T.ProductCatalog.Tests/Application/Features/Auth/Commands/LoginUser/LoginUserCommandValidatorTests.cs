using FluentValidation.TestHelper;
using ADR_T.ProductCatalog.Application.Features.Auth.Commands.LoginUser;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Application.Features.Auth.Commands.LoginUser
{
    public class LoginUserCommandValidatorTests
    {
        private readonly LoginUserCommandValidator _validator;

        public LoginUserCommandValidatorTests()
        {
            _validator = new LoginUserCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Username_Is_Empty()
        {
            // Arrange
            var command = new LoginUserCommand("", "password123");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorMessage("El username es requerido.");
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            // Arrange
            var command = new LoginUserCommand("testuser", "");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("El password es requerido.");
        }

        [Fact]
        public void Should_Have_Error_When_Both_Username_And_Password_Are_Empty()
        {
            // Arrange
            var command = new LoginUserCommand("", "");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Username);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Username_And_Password_Are_Provided()
        {
            // Arrange
            var command = new LoginUserCommand("testuser", "password123");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Username);
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }
    }
}
