using ADR_T.ProductCatalog.Application.DTOs.Auth;
using ADR_T.ProductCatalog.Application.Features.Auth.Commands.RegisterUser;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ADR_T.ProductCatalog.Tests.Unit
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _handler = new RegisterUserCommandHandler(_userRepositoryMock.Object, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_Should_RegisterUser_WhenDataIsValid()
        {
            // Arrange
            var command = new RegisterUserCommand("testuser", "test@test.com", "Password123");
            var newUser = new User(Guid.NewGuid(), command.Username, command.Email);

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync(command.Username))
                .ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(command.Email))
                .ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(r => r.RegisterUserAsync(command.Username, command.Email, command.Password))
                .ReturnsAsync(newUser);
            _userRepositoryMock.Setup(r => r.GetRolesAsync(newUser))
                .ReturnsAsync(new List<string> { "User" });
            _tokenServiceMock.Setup(j => j.GenerateToken(newUser.Username, It.IsAny<string[]>()))
                .Returns("fake-jwt-token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AuthResultDto>();
            result.Token.Should().Be("fake-jwt-token");
        }

        [Fact]
        public async Task Handle_Should_ThrowValidationException_WhenUsernameExists()
        {
            var command = new RegisterUserCommand("testuser", "test@test.com", "Password123");
            var existingUser = new User(Guid.NewGuid(), "testuser", "old@test.com");

            _userRepositoryMock.Setup(r => r.RegisterUserAsync(command.Username, command.Email, command.Password))
                .ThrowsAsync(new ValidationException(new Dictionary<string, string[]>
                {
                    { "DuplicateUserName", new[] { $"El nombre de usuario '{command.Username}' ya está en uso." } }
                }));

            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>()
                .Where(ex => ex.Errors.ContainsKey("DuplicateUserName"));
        }
    }
}