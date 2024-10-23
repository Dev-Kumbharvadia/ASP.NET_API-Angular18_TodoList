using System;
using System.Collections.Generic;
using TodoAPI.Models.Entity;

namespace TodoAPI.Models
{
    public class User
    {
        public Guid UserId { get; set; } // Primary key for the Users table
        public string Username { get; set; } // Maps to Username
        public string PasswordHash { get; set; } // Maps to PasswordHash
        public string Email { get; set; } // Maps to Email
        public DateTime CreatedAt { get; set; } // Maps to CreatedAt
        public DateTime UpdatedAt { get; set; } // Maps to UpdatedAt

        // Navigation property for roles (many-to-many relationship)
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        // Navigation property for todo items (one-to-many relationship)
        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();

        // Navigation property for user audits (one-to-many relationship)
        public ICollection<UserAudit> UserAudits { get; set; } = new List<UserAudit>();
    }
}
