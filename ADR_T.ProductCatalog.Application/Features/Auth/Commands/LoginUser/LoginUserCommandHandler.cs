using ADR_T.ProductCatalog.Application.DTOs.Auth;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Auth.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginUserCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindUserByCredentialsAsync(request.Username, request.Password);
        if (user == null)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["Credentials"] = new[] { "Usuario o contraseña inválidos." }
            });
        }

        var roles = await _userRepository.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user.Username, roles.ToArray());

        return new AuthResultDto(token);
    }
}
