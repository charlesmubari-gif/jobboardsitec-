using BCrypt.Net;
using JobBoard.Models;

namespace JobBoard.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext db)
        {
            // If users exist, assume seeded
            if (db.Users.Any()) return;

            var admin = new User
            {
                Email = "admin@jobboard.local",
                DisplayName = "Site Admin",
                Role = Role.Admin,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
            };

            var employer = new User
            {
                Email = "employer@company.com",
                DisplayName = "Acme Employer",
                Role = Role.Employer,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employer123!")
            };

            var applicant = new User
            {
                Email = "applicant@example.com",
                DisplayName = "Test Applicant",
                Role = Role.Applicant,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Applicant123!")
            };

            db.Users.AddRange(admin, employer, applicant);
            db.SaveChanges();

            var job1 = new Job
            {
                Title = "Backend Developer",
                Description = "Work on APIs and microservices (C#/.NET).",
                Location = "Remote",
                Category = "Software",
                Salary = "Negotiable",
                Deadline = DateTime.UtcNow.AddDays(30),
                PostedAt = DateTime.UtcNow,
                EmployerId = employer.Id
            };

            var job2 = new Job
            {
                Title = "Frontend Developer",
                Description = "React/Blazor developer.",
                Location = "Nairobi, Kenya",
                Category = "Software",
                Salary = "4000-6000",
                Deadline = DateTime.UtcNow.AddDays(20),
                PostedAt = DateTime.UtcNow,
                EmployerId = employer.Id
            };

            db.Jobs.AddRange(job1, job2);
            db.SaveChanges();
        }
    }
}
