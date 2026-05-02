using Lab3.Data;
using Lab3.Helpers;
using Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Controllers
{
    public class PairwiseСomparisonController : Controller
    {
        private readonly AppDbContext _context;

        public PairwiseСomparisonController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Compare(int step = 0)
        {
            var cards = _context.Cards.ToList();

            var pairs = HttpContext.Session.GetObject<List<ComparePair>>("pairs");

            if (pairs == null)
            {
                pairs = new List<ComparePair>();

                for (int i = 0; i < cards.Count; i++)
                {
                    for (int j = i + 1; j < cards.Count; j++)
                    {
                        pairs.Add(new ComparePair
                        {
                            Card1 = cards[i].CardID,
                            Card2 = cards[j].CardID
                        });
                    }
                }

                HttpContext.Session.SetObject("pairs", pairs);
            }

            if (step >= pairs.Count)
                return RedirectToAction("Result");

            var pair = pairs[step];

            var card1 = _context.Cards.First(c => c.CardID == pair.Card1);
            var card2 = _context.Cards.First(c => c.CardID == pair.Card2);

            var effects1 = _context.CardEffects
                .Include(x => x.Effect)
                .Where(x => x.CardID == card1.CardID)
                .ToList();

            var effects2 = _context.CardEffects
                .Include(x => x.Effect)
                .Where(x => x.CardID == card2.CardID)
                .ToList();

            var model = new CompareViewModel
            {
                Card1 = card1,
                Card2 = card2,
                Effects1 = effects1,
                Effects2 = effects2,
                Step = step
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Vote(int winner, int step)
        {
            var votes = HttpContext.Session.GetObject<List<int>>("votes")
                         ?? new List<int>();

            votes.Add(winner);

            HttpContext.Session.SetObject("votes", votes);

            return RedirectToAction("Compare", new { step = step + 1 });
        }

        public IActionResult Result()
        {
            var userId = AuthHelper.GetUserId(HttpContext);

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var votes = HttpContext.Session.GetObject<List<int>>("votes");
            var cards = _context.Cards.ToList();

            int n = cards.Count;

            // IMPORTANT -> double
            double[,] adjacency = new double[n, n];

            int comparisons = n * (n - 1) / 2;
            int[,] incidence = new int[n, comparisons];

            int index = 0;
            int col = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (votes == null || index >= votes.Count)
                    {
                        col++;
                        continue;
                    }

                    var vote = votes[index++];

                    // left win
                    if (vote == 1)
                    {
                        adjacency[i, j] = 1;

                        incidence[i, col] = 1;
                        incidence[j, col] = -1;
                    }
                    // right win
                    else if (vote == 2)
                    {
                        adjacency[j, i] = 1;

                        incidence[i, col] = -1;
                        incidence[j, col] = 1;
                    }
                    // equal
                    else if (vote == 0)
                    {
                        adjacency[i, j] = 0.5;
                        adjacency[j, i] = 0.5;

                        incidence[i, col] = 0;
                        incidence[j, col] = 0;
                    }
                    // cannot compare (9)
                    else if (vote == 9)
                    {
                        // nothing
                    }

                    col++;
                }
            }

            // =============================
            // WINS
            // =============================
            double[] wins = new double[n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    wins[i] += adjacency[i, j];

            var ranking = cards
                .Select((c, i) => new
                {
                    Card = c,
                    Wins = wins[i]
                })
                .OrderByDescending(x => x.Wins)
                .Select((x, index) => new
                {
                    CardID = x.Card.CardID,
                    CardName = x.Card.CardName,
                    Rank = index + 1
                })
                .ToList();


            // =============================
            // SAVE TO DB
            // =============================
            var old = _context.CardRankings
                .Where(x => x.UserID == userId.Value)
                .ToList();

            _context.CardRankings.RemoveRange(old);

            foreach (var r in ranking)
            {
                _context.CardRankings.Add(new CardRanking
                {
                    UserID = userId.Value,
                    CardID = r.CardID,
                    AlternativeRank = r.Rank
                });
            }

            _context.SaveChanges();


            ViewBag.Adjacency = adjacency;
            ViewBag.Incidence = incidence;
            ViewBag.Cards = cards;
            ViewBag.Comparisons = comparisons;
            ViewBag.Ranking = ranking;

            HttpContext.Session.Remove("votes");
            HttpContext.Session.Remove("pairs");

            return View("Matrix");
        }
    }
}