using Microsoft.AspNetCore.Mvc;
using WebApplication5.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AuthController(AppDbContext db) => _db = db;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.UserName == dto.UserName))
                return BadRequest(new { error = "Логин занят" });

            _db.Users.Add(new User
            {
                UserName = dto.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            });
            await _db.SaveChangesAsync();
            return Ok(new { userName = dto.UserName });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { error = "Неверный логин или пароль" });

            return Ok(new { userName = user.UserName });
        }
    }

    public record AuthDto(string UserName, string Password);
}
