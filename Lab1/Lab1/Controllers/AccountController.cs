using Lab1.Constants;
using Lab1.Data;
using Lab1.Filters;
using Lab1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab1.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // REGISTER
        [RedirectIfAuthenticated]
        public IActionResult Register()
        {
            ViewBag.Evaluators = _context.Evaluators.ToList();
            return View();
        }

        [HttpPost]
        [RedirectIfAuthenticated]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Evaluators = _context.Evaluators.ToList();
                return View(user);
            }

            if (_context.Users.Any(u => u.Username == user.Username))
            {
                ViewBag.Error = "Username already exists";
                ViewBag.Evaluators = _context.Evaluators.ToList();
                return View(user);
            }

            user.Role = Roles.User;

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                ViewBag.Error = "Registration error";
                ViewBag.Evaluators = _context.Evaluators.ToList();
                return View(user);
            }

            return RedirectToAction("Login");
        }

        // LOGIN
        [RedirectIfAuthenticated]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [RedirectIfAuthenticated]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserID", user.UserID);
                HttpContext.Session.SetString("Username", user.Username);

                if (user.EvaluatorID.HasValue)
                    HttpContext.Session.SetInt32("EvaluatorID", user.EvaluatorID.Value);
                else
                    HttpContext.Session.Remove("EvaluatorID");

                HttpContext.Session.SetString("Role", user.Role);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Incorrect username or password";
            return View();
        }

        [AuthorizeSession]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
