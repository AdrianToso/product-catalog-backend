using ADR_T.ProductCatalog.Application.DTOs.Auth;
using ADR_T.ProductCatalog.Application.Features.Auth.Commands.LoginUser;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ADR_T.ProductCatalog.Tests.Unit
{
    public class LoginUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenService> _jwtTokenServiceMock;
        private readonly LoginUserCommandHandler _handler;

        public LoginUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtTokenServiceMock = new Mock<ITokenService>();
            _handler = new LoginUserCommandHandler(_userRepositoryMock.Object, _jwtTokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Login_WhenCredentialsValid()
        {
            var command = new LoginUserCommand("testuser", "Password123");
            var user = new User(Guid.NewGuid(), "testuser", "test@test.com");

            _userRepositoryMock.Setup(r => r.FindUserByCredentialsAsync(command.Username, command.Password))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "User" });
            _jwtTokenServiceMock.Setup(j => j.GenerateToken(user.Username, It.IsAny<string[]>()))
                .Returns("fake-jwt-token");

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().BeOfType<AuthResultDto>();
            result.Token.Should().Be("fake-jwt-token");
            result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task Handle_Should_ThrowValidationException_WhenCredentialsInvalid()
        {
            var command = new LoginUserCommand("wronguser", "wrongpass");

            _userRepositoryMock.Setup(r => r.FindUserByCredentialsAsync(command.Username, command.Password))
                .ReturnsAsync((User?)null);

            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>()
                .Where(ex => ex.Errors.ContainsKey("Credentials") && ex.Errors["Credentials"][0] == "Usuario o contraseña inválidos.");
        }
    }
}