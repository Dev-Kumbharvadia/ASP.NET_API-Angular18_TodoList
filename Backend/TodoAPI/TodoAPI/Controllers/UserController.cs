using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using TodoAPI.Models;
using TodoAPI.Data;
using TodoAPI.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TodoAPI.Controllers
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
        public ActionResult<ApiResponse<object>> Login([FromBody] UserLoginModel model)
        {
            var user = _context.Users.Include(u => u.RefreshTokens).SingleOrDefault(u => u.Username == model.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Message = "Invalid credentials",
                    Success = false,
                    Data = null
                });
            }

            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                UserId = user.UserId
            });

            _context.SaveChanges();

            return Ok(new ApiResponse<object>
            {
                Message = "Login successful",
                Success = true,
                Data = new
                {
                    JwtToken = jwtToken,
                    RefreshToken = refreshToken,
                    UserId = user.UserId
                }
            });
        }

        // POST: api/user/refresh-token
        [HttpPost("refresh-token")]
        public ActionResult<ApiResponse<string>> RefreshToken([FromBody] string refreshToken)
        {
            var user = _context.Users.Include(u => u.RefreshTokens).SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user == null)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid refresh token"
                });
            }

            var storedToken = user.RefreshTokens.Single(t => t.Token == refreshToken);

            if (!storedToken.IsActive)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Token expired or revoked"
                });
            }

            // Generate new JWT and refresh token
            var newJwtToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Revoke the old refresh token
            storedToken.Revoked = DateTime.UtcNow;

            // Save the new refresh token to the database
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                UserId = user.UserId
            });

            _context.SaveChanges();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Token refreshed successfully",
                Data = newJwtToken
            });
        }

        // Generate JWT token
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

        // Generate refresh token
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
    }
}
