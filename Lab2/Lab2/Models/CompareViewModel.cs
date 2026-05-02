namespace Lab2.Models
{
    public class CompareViewModel
    {
        public Card Card1 { get; set; } = null!;
        public Card Card2 { get; set; } = null!;

        public List<CardEffect> Effects1 { get; set; } = new();
        public List<CardEffect> Effects2 { get; set; } = new();

        public int Step { get; set; }
    }
}