using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab3.Data;
using Lab3.Models;
using Lab3.Filters;

namespace Lab3.Controllers
{
    [AdminOnly]
    public class CardEffectsController : Controller
    {
        private readonly AppDbContext _context;

        public CardEffectsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CardEffects
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? minValue)
        {
            ViewData["NameSort"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var cardEffects = _context.CardEffects
                .Include(c => c.Card)
                .Include(c => c.Effect)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                cardEffects = cardEffects.Where(c =>
                    c.Card.CardName.Contains(searchString) ||
                    c.Effect.EffectName.Contains(searchString));
            }

            if (minValue.HasValue)
            {
                cardEffects = cardEffects.Where(c => c.Value >= minValue);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    cardEffects = cardEffects.OrderByDescending(c => c.Card.CardName);
                    break;
                default:
                    cardEffects = cardEffects.OrderBy(c => c.Card.CardName);
                    break;
            }

            return View(await cardEffects.ToListAsync());
        }

        // GET: CardEffects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cardEffect = await _context.CardEffects
                .Include(c => c.Card)
                .Include(c => c.Effect)
                .FirstOrDefaultAsync(m => m.CardEffectID == id);

            if (cardEffect == null) return NotFound();

            return View(cardEffect);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewData["CardID"] = new SelectList(_context.Cards, "CardID", "CardName");
            ViewData["EffectID"] = new SelectList(_context.Effects, "EffectID", "EffectName");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CardEffect cardEffect)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cardEffect);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CardID"] = new SelectList(_context.Cards, "CardID", "CardName", cardEffect.CardID);
            ViewData["EffectID"] = new SelectList(_context.Effects, "EffectID", "EffectName", cardEffect.EffectID);

            return View(cardEffect);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cardEffect = await _context.CardEffects.FindAsync(id);
            if (cardEffect == null) return NotFound();

            ViewData["CardID"] = new SelectList(_context.Cards, "CardID", "CardName", cardEffect.CardID);
            ViewData["EffectID"] = new SelectList(_context.Effects, "EffectID", "EffectName", cardEffect.EffectID);

            return View(cardEffect);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CardEffect cardEffect)
        {
            if (id != cardEffect.CardEffectID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(cardEffect);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(cardEffect);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cardEffect = await _context.CardEffects
                .Include(c => c.Card)
                .Include(c => c.Effect)
                .FirstOrDefaultAsync(m => m.CardEffectID == id);

            if (cardEffect == null) return NotFound();

            return View(cardEffect);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cardEffect = await _context.CardEffects.FindAsync(id);
            if (cardEffect != null)
                _context.CardEffects.Remove(cardEffect);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}