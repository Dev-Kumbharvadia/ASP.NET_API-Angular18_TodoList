using System;

namespace TodoAPI.Models
{
    public class TodoItem
    {
        public Guid Id { get; set; } // Primary key for the TodoItems table
        public string Title { get; set; } // Maps to Title
        public string Description { get; set; } // Maps to Description
        public bool IsCompleted { get; set; } // Maps to IsCompleted
        public DateTime? DueDate { get; set; } // Maps to DueDate
        public DateTime CreatedAt { get; set; } // Maps to CreatedAt
        public DateTime UpdatedAt { get; set; } // Maps to UpdatedAt

        // Foreign key for User
        public Guid? UserId { get; set; } // Foreign key for User (nullable)
        public User User { get; set; } // Navigation property to User
    }
}
