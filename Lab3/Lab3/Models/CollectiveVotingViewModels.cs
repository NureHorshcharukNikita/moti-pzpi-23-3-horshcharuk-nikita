namespace Lab3.Models
{
    public class VotingProfileRowViewModel
    {
        public string Username { get; set; } = "";
        /// <summary>Card names at rank 1, 2, … (length = number of alternatives).</summary>
        public IReadOnlyList<string> RankedCardNames { get; set; } = Array.Empty<string>();
    }

    public class BordaScoreRowViewModel
    {
        public string CardName { get; set; } = "";
        public double TotalScore { get; set; }
    }

    /// <summary>
    /// One column in the aggregated Borda voting profile: voters sharing the same ranking (header = count).
    /// </summary>
    public class BordaAggregatedColumnViewModel
    {
        public int VoterCount { get; set; }
        /// <summary>Card names from rank 1 (best) to rank N.</summary>
        public IReadOnlyList<string> CardsFromBestToWorst { get; set; } = Array.Empty<string>();
    }

    public class BordaResultViewModel
    {
        /// <summary>Grouped identical rankings; table columns before the Points column.</summary>
        public IReadOnlyList<BordaAggregatedColumnViewModel> AggregatedProfileColumns { get; set; } =
            Array.Empty<BordaAggregatedColumnViewModel>();

        public IReadOnlyList<BordaScoreRowViewModel> Rows { get; set; } = Array.Empty<BordaScoreRowViewModel>();
        public string? WinnerName { get; set; }
        public int AlternativeCount { get; set; }
    }

    public class PairwiseDuelViewModel
    {
        public string CardA { get; set; } = "";
        public string CardB { get; set; } = "";
        public int VotesPreferA { get; set; }
        public int VotesPreferB { get; set; }
    }

    public class CopelandScoreRowViewModel
    {
        public string CardName { get; set; } = "";
        public int CopelandScore { get; set; }
    }

    public class CopelandResultViewModel
    {
        public IReadOnlyList<PairwiseDuelViewModel> Duels { get; set; } = Array.Empty<PairwiseDuelViewModel>();
        public IReadOnlyList<CopelandScoreRowViewModel> Scores { get; set; } = Array.Empty<CopelandScoreRowViewModel>();
        public string? WinnerName { get; set; }
    }

    public class CollectiveVotingPageViewModel
    {
        public int ProfileCount { get; set; }
        public int AlternativeCount { get; set; }
        public bool HasData { get; set; }
        public string? WarningMessage { get; set; }
    }
}
