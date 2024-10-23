using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;
using TodoAPI.Models.Entity;

namespace TodoAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet properties for each entity
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<UserAudit> UserAudits { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; } // Add RefreshToken entity

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define composite key for UserRoles (many-to-many)
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // Set up one-to-many relationship between User and UserAudit
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserAudits)
                .WithOne(ua => ua.User)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete: remove user and their audits

            // Set up one-to-many relationship between User and TodoItem
            modelBuilder.Entity<User>()
                .HasMany(u => u.TodoItems)
                .WithOne(ti => ti.User)
                .HasForeignKey(ti => ti.UserId)
                .OnDelete(DeleteBehavior.SetNull); // Set user to null but retain todo items

            // Set up one-to-many relationship between Role and UserRole
            modelBuilder.Entity<Role>()
                .HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete: remove role and its associations

            // Set up one-to-many relationship between User and RefreshToken
            modelBuilder.Entity<User>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Remove refresh tokens when user is deleted

            // Optional: configure table names or relationships further if needed
        }
    }
}
