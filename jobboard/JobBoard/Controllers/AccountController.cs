using BCrypt.Net;
using JobBoard.Data;
using JobBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        public AccountController(AppDbContext db) => _db = db;

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string email, string displayName, string password, string role = "Applicant")
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email and password required.");
                return View();
            }

            if (await _db.Users.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError("", "Email already in use.");
                return View();
            }

            var user = new User
            {
                Email = email,
                DisplayName = displayName ?? email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = Enum.TryParse<Role>(role, true, out var r) ? r : Role.Applicant
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // login
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            HttpContext.Session.SetString("UserName", user.DisplayName);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            HttpContext.Session.SetString("UserName", user.DisplayName);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
