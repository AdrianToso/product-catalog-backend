namespace ADR_T.ProductCatalog.Core.Domain.Interfaces;
public interface ITokenService
{
    string GenerateToken(string username, string[] roles);
}