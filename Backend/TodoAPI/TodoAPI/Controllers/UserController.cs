using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net; // Ensure you have installed the BCrypt.Net-Next package
using TodoAPI.Models;
using TodoAPI.Data;
using TodoAPI.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;

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

            // Log user login into the UserAudit table
            var userAudit = new UserAudit
            {
                UserId = user.UserId,
                LoginTime = DateTime.Now,
                LogoutTime = null
            };

            _context.UserAudits.Add(userAudit);
            _context.SaveChanges(); // Save the audit record to the database

            return Ok(new ApiResponse<string>
            {
                Message = Convert.ToString(user.UserId),
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
