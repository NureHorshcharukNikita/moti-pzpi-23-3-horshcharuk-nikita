using Lab3.Data;
using Lab3.Filters;
using Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Controllers
{
    [AdminOnly]
    public class EffectsController : Controller
    {
        private readonly AppDbContext _context;

        public EffectsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Effects
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            ViewData["NameSort"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var effects = from e in _context.Effects
                          select e;

            if (!string.IsNullOrEmpty(searchString))
            {
                effects = effects.Where(e =>
                    e.EffectName.Contains(searchString) ||
                    e.EffectCode.Contains(searchString) ||
                    e.Type.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    effects = effects.OrderByDescending(e => e.EffectName);
                    break;
                default:
                    effects = effects.OrderBy(e => e.EffectName);
                    break;
            }

            return View(await effects.ToListAsync());
        }

        // GET: Effects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var effect = await _context.Effects
                .FirstOrDefaultAsync(m => m.EffectID == id);

            if (effect == null)
            {
                return NotFound();
            }

            return View(effect);
        }

        // GET: Effects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Effects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EffectID,EffectCode,EffectName,EffectDescription,Etalon,Type")] Effect effect)
        {
            if (ModelState.IsValid)
            {
                _context.Add(effect);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(effect);
        }

        // GET: Effects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var effect = await _context.Effects.FindAsync(id);
            if (effect == null)
            {
                return NotFound();
            }

            return View(effect);
        }

        // POST: Effects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EffectID,EffectCode,EffectName,EffectDescription,Etalon,Type")] Effect effect)
        {
            if (id != effect.EffectID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(effect);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EffectExists(effect.EffectID))
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

            return View(effect);
        }

        // GET: Effects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var effect = await _context.Effects
                .FirstOrDefaultAsync(m => m.EffectID == id);

            if (effect == null)
            {
                return NotFound();
            }

            return View(effect);
        }

        // POST: Effects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var effect = await _context.Effects.FindAsync(id);

            if (effect == null)
                return NotFound();

            var isUsed = await _context.CardEffects
                .AnyAsync(ce => ce.EffectID == id);

            if (isUsed)
            {
                TempData["Error"] = "Cannot delete effect because it is used in table: CardEffects";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.Effects.Remove(effect);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool EffectExists(int id)
        {
            return _context.Effects.Any(e => e.EffectID == id);
        }
    }
}