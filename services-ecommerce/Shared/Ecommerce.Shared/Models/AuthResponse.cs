using Ecommerce.Shared.DTOs;

namespace Ecommerce.Shared.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public UserDto User { get; set; }
    }
}