using MediatR;
using ADR_T.ProductCatalog.Application.DTOs.Auth;

namespace ADR_T.ProductCatalog.Application.Features.Auth.Commands.LoginUser;
public sealed record LoginUserCommand(string Username, string Password) : IRequest<AuthResultDto>;
