namespace test1.Models
{
    public class UserRole
    {
        public int UserId { get; set; } // Foreign key for User
        public User User { get; set; }  // Navigation property to User

        public int RoleId { get; set; } // Foreign key for Role
        public Role Role { get; set; }  // Navigation property to Role
    }
}
