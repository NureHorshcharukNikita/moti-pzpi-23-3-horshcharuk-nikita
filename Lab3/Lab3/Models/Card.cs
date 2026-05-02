using System.ComponentModel.DataAnnotations;

namespace Lab3.Models
{
    public class Card
    {
        [Key]
        public int CardID { get; set; }

        [Required]
        public string CardName { get; set; } = "";

        [Required]
        public string CardDescription { get; set; } = "";

        public ICollection<CardEffect>? CardEffects { get; set; }
        public ICollection<CardRanking>? CardRankings { get; set; }
    }
}