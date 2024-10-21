using System;
using System.Collections.Generic;

namespace test1.Models
{
    public class User
    {
        public int UserId { get; set; } // Maps to UserId in the Users table
        public string Username { get; set; } // Maps to Username
        public string PasswordHash { get; set; } // Maps to PasswordHash
        public string Email { get; set; } // Maps to Email
        public DateTime CreatedAt { get; set; } // Maps to CreatedAt
        public DateTime UpdatedAt { get; set; } // Maps to UpdatedAt

        // Navigation property for roles (many-to-many relationship)
        public ICollection<UserRole> UserRoles { get; set; } // A user can have multiple roles

        // Navigation property for todo items (one-to-many relationship)
        public ICollection<TodoItem> TodoItems { get; set; } // A user can have many todo items
    }
}
