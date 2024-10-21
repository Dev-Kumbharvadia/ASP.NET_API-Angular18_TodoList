using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net; // Ensure you have installed the BCrypt.Net-Next package
using test1.Models;
using test1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace test1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public UserController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/user/register
        [HttpPost("register")]
        public ActionResult<ApiResponse<User>> Register([FromBody] UserRegisterModel model)
        {
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                return BadRequest(new ApiResponse<User>
                {
                    Message = "Username already exists",
                    Success = false,
                    Data = null
                });
            }

            var user = new User
            {
                UserId = 0, // The database will auto-increment this
                Username = model.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Email = model.Email,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new ApiResponse<User>
            {
                Message = "User registered successfully",
                Success = true,
                Data = user
            });
        }

        // POST: api/user/login
        [HttpPost("login")]
        public ActionResult<ApiResponse<string>> Login([FromBody] UserLoginModel model)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == model.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Message = "Invalid credentials",
                    Success = false,
                    Data = null
                });
            }

            var token = GenerateJwtToken(user);

            return Ok(new ApiResponse<string>
            {
                Message = "Login successful",
                Success = true,
                Data = token
            });
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_config["Jwt:ExpiresInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
