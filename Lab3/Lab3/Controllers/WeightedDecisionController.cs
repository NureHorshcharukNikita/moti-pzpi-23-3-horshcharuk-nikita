using Lab3.Data;
using Lab3.Helpers;
using Lab3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers
{
    public class WeightedDecisionController : Controller
    {
        private readonly AppDbContext _context;

        public WeightedDecisionController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Criteria()
        {
            var effects = _context.Effects.ToList();
            return View(effects);
        }

        [HttpPost]
        public IActionResult Calculate()
        {
            var userId = AuthHelper.GetUserId(HttpContext);

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cards = _context.Cards.ToList();
            var effects = _context.Effects.ToList();

            // =========================
            // RAW WEIGHTS
            // =========================
            var weights = new Dictionary<int, double>();

            foreach (var e in effects)
            {
                var key = $"w_{e.EffectID}";
                weights[e.EffectID] = double.Parse(Request.Form[key]);
            }

            // =========================
            // NORMALIZE WEIGHTS
            // =========================
            var sumWeights = weights.Values.Sum();

            var balancedWeights = weights.ToDictionary(
                x => x.Key,
                x => sumWeights == 0 ? 0 : x.Value / sumWeights
            );

            var ranking = new List<WeightedDecisionResultItem>();
            var details = new List<dynamic>();

            // =========================
            // CALCULATION
            // =========================
            foreach (var card in cards)
            {
                var ce = _context.CardEffects
                    .Where(x => x.CardID == card.CardID)
                    .ToList();

                double sum = 0;
                var rows = new List<dynamic>();

                foreach (var c in ce)
                {
                    var effect = effects.First(e => e.EffectID == c.EffectID);

                    var weight = balancedWeights[c.EffectID];
                    var etalon = effect.Etalon;

                    double norm;

                    if (effect.Type == "MIN")
                        norm = etalon / c.Value;
                    else
                        norm = c.Value / etalon;

                    var product = norm * weight;

                    rows.Add(new
                    {
                        Name = effect.EffectName,
                        Value = c.Value,
                        Etalon = etalon,
                        Type = effect.Type,
                        Norm = Math.Round(norm, 3),
                        Weight = Math.Round(weight, 3),
                        Product = Math.Round(product, 3)
                    });

                    sum += product;
                }

                details.Add(new
                {
                    CardName = card.CardName,
                    Rows = rows,
                    Sum = Math.Round(sum, 3)
                });

                ranking.Add(new WeightedDecisionResultItem
                {
                    CardName = card.CardName,
                    Utility = sum
                });
            }

            // =========================
            // RANKING
            // =========================
            ranking = ranking
                .OrderByDescending(x => x.Utility)
                .Select((x, i) => new WeightedDecisionResultItem
                {
                    Rank = i + 1,
                    CardName = x.CardName,
                    Utility = Math.Round(x.Utility, 3)
                })
                .ToList();


            // =========================
            // SAVE TO DATABASE
            // =========================
            var old = _context.CardRankings
                .Where(x => x.UserID == userId.Value)
                .ToList();

            _context.CardRankings.RemoveRange(old);

            foreach (var r in ranking)
            {
                var cardId = cards
                    .First(c => c.CardName == r.CardName)
                    .CardID;

                _context.CardRankings.Add(new CardRanking
                {
                    UserID = userId.Value,
                    CardID = cardId,
                    AlternativeRank = r.Rank
                });
            }

            _context.SaveChanges();


            ViewBag.Weights = balancedWeights;
            ViewBag.Details = details;

            return View("Result", ranking);
        }
    }
}