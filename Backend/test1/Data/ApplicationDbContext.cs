﻿using Microsoft.EntityFrameworkCore;
using test1.Models;
using test1.Models.Entity;

namespace test1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<LoginAuditTable> LoginAuditTables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define composite key for UserRoles
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // Optionally, you can seed roles and users here if needed
            modelBuilder.Entity<LoginAuditTable>()
    .HasOne<User>() // Replace User with your related entity
    .WithMany()
    .HasForeignKey(a => a.UserId);
        }
    }
}