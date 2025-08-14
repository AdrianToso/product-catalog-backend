using ADR_T.ProductCatalog.Core.Domain.Entities;

namespace ADR_T.ProductCatalog.Core.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> RegisterUserAsync(string username, string email, string password);
    Task<User?> FindUserByCredentialsAsync(string username, string password);
    Task<IList<string>> GetRolesAsync(User user);
}