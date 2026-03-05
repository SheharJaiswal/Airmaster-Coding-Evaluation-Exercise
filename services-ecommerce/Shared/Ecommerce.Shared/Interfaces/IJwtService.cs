using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Models;

namespace Ecommerce.Shared.Interfaces
{
    public interface IJwtService
    {
        AuthResponse GenerateToken(User user);
    }
}
