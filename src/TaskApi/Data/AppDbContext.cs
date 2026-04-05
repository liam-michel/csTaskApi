using Microsoft.EntityFrameworkCore;
using TaskApi.Models;

namespace TaskApi.Data;
// Data/AppDbContext.cs
public class AppDbContext : DbContext
{
    public DbSet<TaskItem> Tasks { get; set; }      // Task table
    public DbSet<User> Users { get; set; }      // User table

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define the relationships and database-level constraints
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}