using Lab2.Data;
using Lab2.Filters;
using Lab2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Controllers
{
    [AdminOnly]
    public class EvaluatorsController : Controller
    {
        private readonly AppDbContext _context;

        public EvaluatorsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Evaluators
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? minRank)
        {
            ViewData["NameSort"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var evaluators = from e in _context.Evaluators
                             select e;

            if (!string.IsNullOrEmpty(searchString))
            {
                evaluators = evaluators.Where(e => e.EvaluatorName.Contains(searchString));
            }

            if (minRank.HasValue)
            {
                evaluators = evaluators.Where(e => e.CompetenceRank >= minRank);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    evaluators = evaluators.OrderByDescending(e => e.EvaluatorName);
                    break;
                default:
                    evaluators = evaluators.OrderBy(e => e.EvaluatorName);
                    break;
            }

            return View(await evaluators.ToListAsync());
        }

        // GET: Evaluators/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evaluator = await _context.Evaluators
                .FirstOrDefaultAsync(m => m.EvaluatorID == id);

            if (evaluator == null)
            {
                return NotFound();
            }

            return View(evaluator);
        }

        // GET: Evaluators/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Evaluators/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EvaluatorID,EvaluatorName,CompetenceRank")] Evaluator evaluator)
        {
            if (ModelState.IsValid)
            {
                _context.Add(evaluator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(evaluator);
        }

        // GET: Evaluators/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evaluator = await _context.Evaluators.FindAsync(id);
            if (evaluator == null)
            {
                return NotFound();
            }

            return View(evaluator);
        }

        // POST: Evaluators/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EvaluatorID,EvaluatorName,CompetenceRank")] Evaluator evaluator)
        {
            if (id != evaluator.EvaluatorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evaluator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EvaluatorExists(evaluator.EvaluatorID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(evaluator);
        }

        // GET: Evaluators/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evaluator = await _context.Evaluators
                .FirstOrDefaultAsync(m => m.EvaluatorID == id);

            if (evaluator == null)
            {
                return NotFound();
            }

            return View(evaluator);
        }

        // POST: Evaluators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evaluator = await _context.Evaluators.FindAsync(id);
            if (evaluator != null)
            {
                _context.Evaluators.Remove(evaluator);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EvaluatorExists(int id)
        {
            return _context.Evaluators.Any(e => e.EvaluatorID == id);
        }
    }
}