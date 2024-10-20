using Microsoft.EntityFrameworkCore;
using test1.Models;

namespace test1.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; } // DbSet for TodoItem entity
    }
}
