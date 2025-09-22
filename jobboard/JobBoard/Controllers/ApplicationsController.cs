using JobBoard.Data;
using JobBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ApplicationsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Apply view
        public async Task<IActionResult> Apply(int jobId)
        {
            var job = await _db.Jobs.FindAsync(jobId);
            if (job == null) return NotFound();
            return View(job);
        }

        [HttpPost]
        public async Task<IActionResult> ApplySubmit(int jobId, string coverLetter, IFormFile? cv)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var job = await _db.Jobs.FindAsync(jobId);
            if (job == null) return NotFound();

            string? savedPath = null;
            if (cv != null && cv.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(cv.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                using var fs = new FileStream(filePath, FileMode.Create);
                await cv.CopyToAsync(fs);
                savedPath = $"/uploads/{fileName}";
            }

            var application = new JobApplication
            {
                JobId = jobId,
                ApplicantId = userId.Value,
                CoverLetter = coverLetter,
                CvFilePath = savedPath
            };

            _db.JobApplications.Add(application);
            await _db.SaveChangesAsync();

            return RedirectToAction("MyApplications");
        }

        public async Task<IActionResult> MyApplications()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var apps = await _db.JobApplications
                .Include(a => a.Job)
                .ThenInclude(j => j.Employer)
                .Where(a => a.ApplicantId == userId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

            return View(apps);
        }

        // Employer viewing applicants for their job
        public async Task<IActionResult> ApplicantsForJob(int jobId)
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var job = await _db.Jobs.FindAsync(jobId);
            if (job == null) return NotFound();

            // only employer owner or admin
            var role = HttpContext.Session.GetString("UserRole");
            if (job.EmployerId != userId && role != Role.Admin.ToString()) return Forbid();

            var apps = await _db.JobApplications
                .Include(a => a.Applicant)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

            return View(apps);
        }
    }
}
