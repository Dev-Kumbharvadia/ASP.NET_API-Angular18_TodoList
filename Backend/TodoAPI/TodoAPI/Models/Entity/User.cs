using System;
using System.Collections.Generic;
using TodoAPI.Models.Entity;

namespace TodoAPI.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property for roles (many-to-many relationship)
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        // Navigation property for todo items (one-to-many relationship)
        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();

        // Navigation property for user audits (one-to-many relationship)
        public ICollection<UserAudit> UserAudits { get; set; } = new List<UserAudit>();

        // Navigation property for refresh tokens (one-to-many relationship)
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }

}
