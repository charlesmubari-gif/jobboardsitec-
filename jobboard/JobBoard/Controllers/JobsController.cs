using JobBoard.Data;
using JobBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Controllers
{
    public class JobsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public JobsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Public list
        public async Task<IActionResult> Index()
        {
            var jobs = await _db.Jobs.Include(j => j.Employer).OrderByDescending(j => j.PostedAt).ToListAsync();
            return View(jobs);
        }

        // Details + apply button
        public async Task<IActionResult> Details(int id)
        {
            var job = await _db.Jobs.Include(j => j.Employer).FirstOrDefaultAsync(j => j.Id == id);
            if (job == null) return NotFound();
            return View(job);
        }

        // Employer - Create
        public IActionResult Create()
        {
            if (!IsLoggedInRole(Role.Employer)) return Forbid();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Job model)
        {
            if (!IsLoggedInRole(Role.Employer)) return Forbid();

            if (!ModelState.IsValid) return View(model);

            var userId = GetUserId();
            model.EmployerId = userId;
            model.PostedAt = DateTime.UtcNow;
            _db.Jobs.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Employer - Edit
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedInRole(Role.Employer)) return Forbid();

            var job = await _db.Jobs.FindAsync(id);
            if (job == null) return NotFound();
            if (job.EmployerId != GetUserId() && !IsLoggedInRole(Role.Admin)) return Forbid();
            return View(job);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Job model)
        {
            if (!IsLoggedInRole(Role.Employer)) return Forbid();

            var job = await _db.Jobs.FindAsync(id);
            if (job == null) return NotFound();
            if (job.EmployerId != GetUserId() && !IsLoggedInRole(Role.Admin)) return Forbid();

            job.Title = model.Title;
            job.Description = model.Description;
            job.Location = model.Location;
            job.Category = model.Category;
            job.Salary = model.Salary;
            job.Deadline = model.Deadline;
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new { id = job.Id });
        }

        // Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedInRole(Role.Employer)) return Forbid();

            var job = await _db.Jobs.FindAsync(id);
            if (job == null) return NotFound();
            if (job.EmployerId != GetUserId() && !IsLoggedInRole(Role.Admin)) return Forbid();

            _db.Jobs.Remove(job);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Helper
        private int GetUserId()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }

        private bool IsLoggedInRole(Role role)
        {
            var s = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(s)) return false;
            return Enum.TryParse<Role>(s, out var r) && r == role || s == Role.Admin.ToString();
        }
    }
}
