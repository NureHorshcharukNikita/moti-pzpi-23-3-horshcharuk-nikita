namespace Lab2.Models
{
    public class WeightedDecisionResultItem
    {
        public int Rank { get; set; }
        public string CardName { get; set; } = "";
        public double Utility { get; set; }
    }
}