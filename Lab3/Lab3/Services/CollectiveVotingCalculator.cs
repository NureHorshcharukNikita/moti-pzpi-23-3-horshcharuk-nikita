using Lab3.Models;

namespace Lab3.Services
{
    public sealed record CardAlt(int Id, string Name);

    public sealed class UserRankProfile
    {
        public int UserId { get; init; }
        public string Username { get; init; } = "";
        public IReadOnlyDictionary<int, int> CardIdToRank { get; init; } =
            new Dictionary<int, int>();
    }

    public static class CollectiveVotingCalculator
    {
        /// <summary>
        /// Keeps only users with exactly N ranks over all N cards and ranks forming a permutation of 1…N.
        /// </summary>
        public static IReadOnlyList<UserRankProfile> GetValidProfiles(
            IReadOnlyList<CardAlt> allCards,
            IEnumerable<CardRanking> rankings)
        {
            var cardIdSet = allCards.Select(c => c.Id).ToHashSet();
            int n = allCards.Count;
            if (n == 0)
                return Array.Empty<UserRankProfile>();

            var list = new List<UserRankProfile>();

            foreach (var group in rankings.GroupBy(r => r.UserID))
            {
                var rows = group.ToList();
                if (rows.Count != n)
                    continue;

                var map = rows.ToDictionary(r => r.CardID, r => r.AlternativeRank);
                if (map.Count != n || map.Keys.Any(id => !cardIdSet.Contains(id)))
                    continue;

                var sortedRanks = map.Values.OrderBy(x => x).ToList();
                if (!Enumerable.SequenceEqual(sortedRanks, Enumerable.Range(1, n)))
                    continue;

                var username = rows.First().User?.Username?.Trim();
                if (string.IsNullOrEmpty(username))
                    username = $"User #{group.Key}";

                list.Add(new UserRankProfile
                {
                    UserId = group.Key,
                    Username = username,
                    CardIdToRank = map
                });
            }

            return list;
        }

        public static IReadOnlyList<VotingProfileRowViewModel> ToProfileRows(
            IReadOnlyList<CardAlt> allCards,
            IReadOnlyList<UserRankProfile> profiles)
        {
            int n = allCards.Count;
            var rows = new List<VotingProfileRowViewModel>();

            foreach (var p in profiles.OrderBy(x => x.Username, StringComparer.OrdinalIgnoreCase))
            {
                var namesByRank = new string[n];
                for (int rank = 1; rank <= n; rank++)
                {
                    var cardId = p.CardIdToRank.First(kv => kv.Value == rank).Key;
                    var name = allCards.First(c => c.Id == cardId).Name;
                    namesByRank[rank - 1] = name;
                }

                rows.Add(new VotingProfileRowViewModel
                {
                    Username = p.Username,
                    RankedCardNames = namesByRank
                });
            }

            return rows;
        }

        /// <summary>
        /// Groups voters with identical full rankings; each column is one pattern, header = number of voters.
        /// </summary>
        public static IReadOnlyList<BordaAggregatedColumnViewModel> BuildBordaAggregatedProfile(
            IReadOnlyList<CardAlt> allCards,
            IReadOnlyList<UserRankProfile> profiles)
        {
            int n = allCards.Count;
            if (n == 0 || profiles.Count == 0)
                return Array.Empty<BordaAggregatedColumnViewModel>();

            var buckets = new Dictionary<string, (string[] Pattern, int Count)>(StringComparer.Ordinal);

            foreach (var p in profiles)
            {
                var names = new string[n];
                for (var rank = 1; rank <= n; rank++)
                {
                    var cardId = p.CardIdToRank.First(kv => kv.Value == rank).Key;
                    names[rank - 1] = allCards.First(c => c.Id == cardId).Name;
                }

                var key = string.Join('\u001f', names);
                if (buckets.TryGetValue(key, out var existing))
                    buckets[key] = (existing.Pattern, existing.Count + 1);
                else
                    buckets[key] = ((string[])names.Clone(), 1);
            }

            return buckets
                .OrderByDescending(kv => kv.Value.Count)
                .ThenBy(kv => kv.Key, StringComparer.Ordinal)
                .Select(kv => new BordaAggregatedColumnViewModel
                {
                    VoterCount = kv.Value.Count,
                    CardsFromBestToWorst = kv.Value.Pattern
                })
                .ToList();
        }

        /// <summary>
        /// Borda rule: rank k (1 = best) earns (N − k) points for N alternatives.
        /// </summary>
        public static BordaResultViewModel ComputeBorda(
            IReadOnlyList<CardAlt> allCards,
            IReadOnlyList<UserRankProfile> profiles)
        {
            int n = allCards.Count;
            var totals = allCards.ToDictionary(c => c.Id, _ => 0.0);

            foreach (var p in profiles)
            {
                foreach (var c in allCards)
                {
                    var rank = p.CardIdToRank[c.Id];
                    totals[c.Id] += n - rank;
                }
            }

            var ordered = allCards
                .Select(c => new BordaScoreRowViewModel
                {
                    CardName = c.Name,
                    TotalScore = totals[c.Id]
                })
                .OrderByDescending(r => r.TotalScore)
                .ThenBy(r => r.CardName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var max = ordered.FirstOrDefault()?.TotalScore ?? 0;
            var winners = ordered.Where(r => Math.Abs(r.TotalScore - max) < 1e-9).Select(r => r.CardName).ToList();
            var winner = winners.Count switch
            {
                0 => null,
                1 => winners[0],
                _ => string.Join(", ", winners)
            };

            var aggregated = BuildBordaAggregatedProfile(allCards, profiles);

            return new BordaResultViewModel
            {
                AggregatedProfileColumns = aggregated,
                Rows = ordered,
                WinnerName = winner,
                AlternativeCount = n
            };
        }

        /// <summary>
        /// Copeland rule: +1 / 0 / −1 for a win / tie / loss in each pairwise duel.
        /// </summary>
        public static CopelandResultViewModel ComputeCopeland(
            IReadOnlyList<CardAlt> allCards,
            IReadOnlyList<UserRankProfile> profiles)
        {
            var cards = allCards.OrderBy(c => c.Id).ToList();
            var copeland = cards.ToDictionary(c => c.Id, _ => 0);
            var duels = new List<PairwiseDuelViewModel>();

            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = i + 1; j < cards.Count; j++)
                {
                    var a = cards[i];
                    var b = cards[j];
                    int va = 0, vb = 0;

                    foreach (var p in profiles)
                    {
                        if (p.CardIdToRank[a.Id] < p.CardIdToRank[b.Id])
                            va++;
                        else if (p.CardIdToRank[b.Id] < p.CardIdToRank[a.Id])
                            vb++;
                    }

                    duels.Add(new PairwiseDuelViewModel
                    {
                        CardA = a.Name,
                        CardB = b.Name,
                        VotesPreferA = va,
                        VotesPreferB = vb
                    });

                    if (va > vb)
                    {
                        copeland[a.Id]++;
                        copeland[b.Id]--;
                    }
                    else if (vb > va)
                    {
                        copeland[b.Id]++;
                        copeland[a.Id]--;
                    }
                }
            }

            var scoreRows = cards
                .Select(c => new CopelandScoreRowViewModel
                {
                    CardName = c.Name,
                    CopelandScore = copeland[c.Id]
                })
                .OrderByDescending(r => r.CopelandScore)
                .ThenBy(r => r.CardName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var top = scoreRows.FirstOrDefault()?.CopelandScore ?? 0;
            var w = scoreRows.Where(r => r.CopelandScore == top).Select(r => r.CardName).ToList();
            var winner = w.Count switch
            {
                0 => null,
                1 => w[0],
                _ => string.Join(", ", w)
            };

            return new CopelandResultViewModel
            {
                Duels = duels,
                Scores = scoreRows,
                WinnerName = winner
            };
        }
    }
}
