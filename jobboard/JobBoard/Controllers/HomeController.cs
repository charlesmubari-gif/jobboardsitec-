using JobBoard.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? q)
        {
            var jobs = _db.Jobs.Include(j => j.Employer).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                jobs = jobs.Where(j => j.Title.Contains(q) || j.Description.Contains(q) || j.Location.Contains(q) || j.Category.Contains(q));
            }

            var model = await jobs.OrderByDescending(j => j.PostedAt).ToListAsync();
            return View(model);
        }
    }
}
