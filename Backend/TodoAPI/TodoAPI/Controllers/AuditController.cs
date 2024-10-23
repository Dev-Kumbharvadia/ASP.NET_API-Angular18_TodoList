using Microsoft.AspNetCore.Mvc;
using TodoAPI.Data;
using TodoAPI.Models.Entity;
using System.Linq;
using System;
using TodoAPI.Models;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuditController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/audit/logout/{userId}
        [HttpPost("logout/{userId}")]
        public ActionResult<ApiResponse<UserAudit>> Logout(Guid userId)
        {
            // Find the last audit entry for the user (the most recent login without a logout time)
            var lastAudit = _context.UserAudits
                .Where(ua => ua.UserId == userId && ua.LogoutTime == null)
                .OrderByDescending(ua => ua.LoginTime)
                .FirstOrDefault();

            if (lastAudit == null)
            {
                return BadRequest(new ApiResponse<UserAudit>
                {
                    Message = "No active login session found for this user",
                    Success = false,
                    Data = null
                });
            }

            // Set the logout time to the current time
            lastAudit.LogoutTime = DateTime.Now;
            _context.SaveChanges(); // Update the audit record in the database

            return Ok(new ApiResponse<UserAudit>
            {
                Message = "Logout recorded successfully",
                Success = true,
                Data = lastAudit
            });
        }
    }
}
