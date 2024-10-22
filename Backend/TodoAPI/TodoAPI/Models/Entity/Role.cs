using System.Collections.Generic;

namespace TodoAPI.Models
{
    public class Role
    {
        public int RoleId { get; set; } // Maps to RoleId in the Roles table
        public string RoleName { get; set; } // Maps to RoleName

        // Navigation property for users (many-to-many relationship)
        public ICollection<UserRole> UserRoles { get; set; } // A role can belong to many users
    }
}
