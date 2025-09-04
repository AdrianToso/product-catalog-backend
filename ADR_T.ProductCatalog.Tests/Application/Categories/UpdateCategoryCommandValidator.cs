using FluentValidation.TestHelper;
using ADR_T.ProductCatalog.Application.Features.Categories.Commands.UpdateCategory;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Application.Categories
{
    public class UpdateCategoryCommandValidatorTests
    {
        private readonly UpdateCategoryCommandValidator _validator;

        public UpdateCategoryCommandValidatorTests()
        {
            _validator = new UpdateCategoryCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            // Arrange
            var command = new UpdateCategoryCommand(Guid.Empty, "Valid Name", "Valid Description");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("El ID es obligatorio.");
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            // Arrange
            var command = new UpdateCategoryCommand(Guid.NewGuid(), "", "Valid Description");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("El nombre es obligatorio.");
        }

        [Fact]
        public void Should_Have_Error_When_Name_Exceeds_Max_Length()
        {
            // Arrange
            var longName = new string('a', 101);
            var command = new UpdateCategoryCommand(Guid.NewGuid(), longName, "Valid Description");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("El nombre no debe exceder los 100 caracteres.");
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_Max_Length()
        {
            // Arrange
            var longDescription = new string('a', 501);
            var command = new UpdateCategoryCommand(Guid.NewGuid(), "Valid Name", longDescription);

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("La descripción no debe exceder los 500 caracteres.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new UpdateCategoryCommand(Guid.NewGuid(), "Valid Name", "Valid Description");

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Null()
        {
            // Arrange
            var command = new UpdateCategoryCommand(Guid.NewGuid(), "Valid Name", null);

            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }
    }
}
