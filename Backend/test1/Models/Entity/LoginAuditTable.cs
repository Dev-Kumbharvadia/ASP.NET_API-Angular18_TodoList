namespace test1.Models.Entity
{
    public class LoginAuditTable
    {
        // Primary key
        public Guid Id { get; set; }

        // Foreign key referencing the User table
        public Guid UserId { get; set; }

        // Navigation property to link the login audit with the User
        public User User { get; set; }

        // Date and time of the login
        public DateTime LoginDate { get; set; }
    }
}
