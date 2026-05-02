using Lab3.Data;
using Lab3.Filters;
using Lab3.Models;
using Lab3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Controllers
{
    [AuthorizeSession]
    public class CollectiveVotingController : Controller
    {
        private readonly AppDbContext _context;

        public CollectiveVotingController(AppDbContext context)
        {
            _context = context;
        }

        private async Task<(IReadOnlyList<CardAlt> cards, IReadOnlyList<UserRankProfile> profiles)> LoadAsync()
        {
            var cards = await _context.Cards
                .AsNoTracking()
                .OrderBy(c => c.CardID)
                .Select(c => new CardAlt(c.CardID, c.CardName))
                .ToListAsync();

            var rankings = await _context.CardRankings
                .AsNoTracking()
                .Include(r => r.User)
                .ToListAsync();

            var profiles = CollectiveVotingCalculator.GetValidProfiles(cards, rankings);
            return (cards, profiles);
        }

        public async Task<IActionResult> Index()
        {
            var (cards, profiles) = await LoadAsync();
            var vm = new CollectiveVotingPageViewModel
            {
                AlternativeCount = cards.Count,
                ProfileCount = profiles.Count,
                HasData = profiles.Count > 0 && cards.Count > 0,
                WarningMessage = profiles.Count == 0 && cards.Count > 0
                    ? "Add at least one full card ranking (all cards, ranks 1…N with no duplicates) to build a voting profile."
                    : cards.Count == 0
                        ? "There are no cards in the database yet — add alternatives in the admin section."
                        : null
            };

            return View(vm);
        }

        public async Task<IActionResult> Profile()
        {
            var (cards, profiles) = await LoadAsync();
            if (cards.Count == 0)
                return RedirectToAction(nameof(Index));

            if (profiles.Count == 0)
            {
                TempData["Warning"] =
                    "No complete ranking profiles yet. Users should save a full ranking under RankCards.";
                return RedirectToAction(nameof(Index));
            }

            var rows = CollectiveVotingCalculator.ToProfileRows(cards, profiles);
            ViewBag.AlternativeCount = cards.Count;
            return View(rows);
        }

        public async Task<IActionResult> Borda()
        {
            var (cards, profiles) = await LoadAsync();
            if (profiles.Count == 0)
            {
                TempData["Warning"] = "Borda calculation requires at least one full ranking profile.";
                return RedirectToAction(nameof(Index));
            }

            var result = CollectiveVotingCalculator.ComputeBorda(cards, profiles);
            return View(result);
        }

        public async Task<IActionResult> Copeland()
        {
            var (cards, profiles) = await LoadAsync();
            if (profiles.Count == 0)
            {
                TempData["Warning"] = "Copeland calculation requires at least one full ranking profile.";
                return RedirectToAction(nameof(Index));
            }

            var result = CollectiveVotingCalculator.ComputeCopeland(cards, profiles);
            return View(result);
        }
    }
}
