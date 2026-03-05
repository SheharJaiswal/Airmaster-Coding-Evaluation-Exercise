using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Interfaces;
using Ecommerce.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IJwtService _jwtService;

    public AuthController(IUserRepository userRepo, IJwtService jwtService)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userRepo.GetByUsernameAsync(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized();

        await _userRepo.UpdateLastLoginAsync(user.Id);
        return Ok(_jwtService.GenerateToken(user));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (await _userRepo.GetByUsernameAsync(request.Username) != null)
            return BadRequest("Username already exists");

        if (await _userRepo.GetByEmailAsync(request.Email) != null)
            return BadRequest("Email already registered");

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        if (string.IsNullOrWhiteSpace(user.Id))
            user.Id = Guid.NewGuid().ToString();

        await _userRepo.CreateAsync(user);
        return Ok(_jwtService.GenerateToken(user));
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst("id").Value;
        return Ok(await _userRepo.GetByIdAsync(userId));
    }
}