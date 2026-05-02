using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab2.Data;
using Lab2.Models;
using Lab2.Filters;

namespace Lab2.Controllers
{
    [AdminOnly]
    public class CardRankingsController : Controller
    {
        private readonly AppDbContext _context;

        public CardRankingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Index
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? maxRank)
        {
            ViewData["NameSort"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var cardRankings = _context.CardRankings
                .Include(c => c.Card)
                .Include(c => c.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                cardRankings = cardRankings.Where(c =>
                    c.Card.CardName.Contains(searchString) ||
                    c.User.Username.Contains(searchString));
            }

            if (maxRank.HasValue)
            {
                cardRankings = cardRankings.Where(c => c.AlternativeRank <= maxRank);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    cardRankings = cardRankings.OrderByDescending(c => c.Card.CardName);
                    break;
                default:
                    cardRankings = cardRankings.OrderBy(c => c.Card.CardName);
                    break;
            }

            return View(await cardRankings.ToListAsync());
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cardRanking = await _context.CardRankings
                .Include(c => c.Card)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.RankingID == id);

            if (cardRanking == null) return NotFound();

            return View(cardRanking);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewData["CardID"] = new SelectList(_context.Cards, "CardID", "CardName");
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Username");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CardRanking cardRanking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cardRanking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CardID"] = new SelectList(_context.Cards, "CardID", "CardName", cardRanking.CardID);
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Username", cardRanking.UserID);

            return View(cardRanking);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cardRanking = await _context.CardRankings.FindAsync(id);
            if (cardRanking == null) return NotFound();

            ViewData["CardID"] = new SelectList(_context.Cards, "CardID", "CardName", cardRanking.CardID);
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Username", cardRanking.UserID);

            return View(cardRanking);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CardRanking cardRanking)
        {
            if (id != cardRanking.RankingID) return NotFound();

            _context.Update(cardRanking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cardRanking = await _context.CardRankings
                .Include(c => c.Card)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.RankingID == id);

            if (cardRanking == null) return NotFound();

            return View(cardRanking);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cardRanking = await _context.CardRankings.FindAsync(id);
            if (cardRanking != null)
                _context.CardRankings.Remove(cardRanking);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}