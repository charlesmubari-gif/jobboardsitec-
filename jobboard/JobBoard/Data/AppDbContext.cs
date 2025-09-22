using JobBoard.Models;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Job> Jobs { get; set; } = null!;
        public DbSet<JobApplication> JobApplications { get; set; } = null!;
    }
}
