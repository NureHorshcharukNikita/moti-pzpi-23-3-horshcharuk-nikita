using Lab1.Data;
using Lab1.Models;
using Lab1.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Lab1.Controllers
{
    [UserOnly]
    public class UserRankingController : Controller
    {
        private readonly AppDbContext _context;

        public UserRankingController(AppDbContext context)
        {
            _context = context;
        }

        // GET
        public IActionResult Index(bool edit = false)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cards = _context.Cards.ToList();

            var existing = _context.CardRankings
                .Where(r => r.UserID == userId)
                .ToList();

            var model = cards.Select(c => new RankInput
            {
                CardID = c.CardID,
                CardName = c.CardName,
                Rank = existing.FirstOrDefault(e => e.CardID == c.CardID)?.AlternativeRank ?? 0
            }).ToList();

            ViewBag.Edit = edit;

            return View(model);
        }

        // POST
        [HttpPost]
        public IActionResult Save(List<RankInput> ranks)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            // унікальність
            if (ranks.Select(r => r.Rank).Distinct().Count() != ranks.Count)
            {
                ModelState.AddModelError("", "Ranks must be unique!");
                RestoreCardNames(ranks);
                ViewBag.Edit = true;
                return View("Index", ranks);
            }

            // повний набір 1..N
            int n = ranks.Count;
            var expected = Enumerable.Range(1, n);

            if (!expected.All(x => ranks.Select(r => r.Rank).Contains(x)))
            {
                ModelState.AddModelError("", "Ranks must be from 1 to N without gaps!");
                RestoreCardNames(ranks);
                ViewBag.Edit = true;
                return View("Index", ranks);
            }

            // видалити старі
            var old = _context.CardRankings
                .Where(r => r.UserID == userId);

            _context.CardRankings.RemoveRange(old);

            // додати нові
            foreach (var r in ranks)
            {
                _context.CardRankings.Add(new CardRanking
                {
                    UserID = userId.Value,
                    CardID = r.CardID,
                    AlternativeRank = r.Rank
                });
            }

            _context.SaveChanges();

            TempData["Success"] = "Ranking saved!";
            return RedirectToAction("Index");
        }

        // helper щоб не зникали назви
        private void RestoreCardNames(List<RankInput> ranks)
        {
            foreach (var r in ranks)
            {
                r.CardName = _context.Cards
                    .First(c => c.CardID == r.CardID)
                    .CardName;
            }
        }
    }
}