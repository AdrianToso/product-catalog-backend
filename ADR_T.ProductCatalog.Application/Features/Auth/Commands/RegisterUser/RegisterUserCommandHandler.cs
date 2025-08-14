using ADR_T.ProductCatalog.Application.DTOs.Auth;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RegisterUserCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        var existingByUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingByUser != null)
            errors["Username"] = new[] { "El usuario ya existe." };

        var existingByEmail = await _userRepository.GetUserByEmailAsync(request.Email);
        if (existingByEmail != null)
            errors["Email"] = new[] { "El email ya está en uso." };

        if (errors.Any())
            throw new ValidationException(errors);

        var user = await _userRepository.RegisterUserAsync(request.Username, request.Email, request.Password);
        if (user == null)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["General"] = new[] { "No se pudo registrar el usuario." }
            });

        var roles = await _userRepository.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user.Username, roles.ToArray());

        return new AuthResultDto(token);
    }
}
