using System.Collections.Generic;

namespace TodoAPI.Models
{
    public class Role
    {
        public Guid RoleId { get; set; } // Primary key for the Roles table
        public string RoleName { get; set; } // Maps to RoleName

        // Navigation property for users (many-to-many relationship)
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
