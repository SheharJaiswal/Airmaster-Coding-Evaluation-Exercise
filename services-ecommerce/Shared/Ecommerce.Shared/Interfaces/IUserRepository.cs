using Ecommerce.Shared.Entities;

namespace Ecommerce.Shared.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task UpdateLastLoginAsync(string userId);
    }
}
