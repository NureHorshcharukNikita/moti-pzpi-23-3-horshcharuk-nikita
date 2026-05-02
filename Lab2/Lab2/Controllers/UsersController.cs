using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab2.Data;
using Lab2.Models;
using Lab2.Filters;

namespace Lab2.Controllers
{
    [AdminOnly]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Users.Include(u => u.Evaluator);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.Evaluator)
                .FirstOrDefaultAsync(m => m.UserID == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["EvaluatorID"] = new SelectList(_context.Evaluators, "EvaluatorID", "EvaluatorName");
            ViewData["Roles"] = new SelectList(new[] { "User", "Admin" });

            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,Username,Password,Role,EvaluatorID")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EvaluatorID"] = new SelectList(_context.Evaluators, "EvaluatorID", "EvaluatorName", user.EvaluatorID);
            ViewData["Roles"] = new SelectList(new[] { "User", "Admin" }, user.Role);

            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewData["EvaluatorID"] = new SelectList(_context.Evaluators, "EvaluatorID", "EvaluatorName", user.EvaluatorID);
            ViewData["Roles"] = new SelectList(new[] { "User", "Admin" }, user.Role);

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,Username,Password,Role,EvaluatorID")] User user)
        {
            if (id != user.UserID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EvaluatorID"] = new SelectList(_context.Evaluators, "EvaluatorID", "EvaluatorName", user.EvaluatorID);
            ViewData["Roles"] = new SelectList(new[] { "User", "Admin" }, user.Role);

            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.Evaluator)
                .FirstOrDefaultAsync(m => m.UserID == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Users/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
                _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
