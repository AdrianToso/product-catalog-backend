using MediatR;
using ADR_T.ProductCatalog.Application.DTOs.Auth;

namespace ADR_T.ProductCatalog.Application.Features.Auth.Commands.RegisterUser;
public sealed record RegisterUserCommand(string Username, string Email, string Password) : IRequest<AuthResultDto>;
