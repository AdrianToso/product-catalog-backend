using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using ADR_T.ProductCatalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace ADR_T.ProductCatalog.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var appUser = await _userManager.FindByNameAsync(username);
            return MapToDomain(appUser);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var appUser = await _userManager.FindByEmailAsync(email);
            return MapToDomain(appUser);
        }

        public async Task<User?> FindUserByCredentialsAsync(string username, string password)
        {
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return null;

            var check = await _signInManager.CheckPasswordSignInAsync(appUser, password, false);
            if (!check.Succeeded) return null;

            return MapToDomain(appUser);
        }

        public async Task<User?> RegisterUserAsync(string username, string email, string password)
        {
            var appUser = new ApplicationUser
            {
                UserName = username,
                Email = email
            };

            var result = await _userManager.CreateAsync(appUser, password);

            if (!result.Succeeded)
            {
                var errorDict = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToArray()
                    );

                throw new ValidationException(errorDict);
            }

            if (await _roleManager.RoleExistsAsync("User"))
            {
                await _userManager.AddToRoleAsync(appUser, "User");
            }

            return MapToDomain(appUser);
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser == null) return new List<string>();
            return await _userManager.GetRolesAsync(appUser);
        }

        private static User? MapToDomain(ApplicationUser? appUser)
            => appUser == null
                ? null
                : new User(Guid.Parse(appUser.Id), appUser.UserName ?? string.Empty, appUser.Email ?? string.Empty);
    }
}
