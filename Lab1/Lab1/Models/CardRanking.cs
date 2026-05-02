using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab1.Models
{
    public class CardRanking
    {
        [Key]
        public int RankingID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int CardID { get; set; }

        [Range(1, int.MaxValue)]
        public int AlternativeRank { get; set; }

        [ForeignKey("UserID")]
        public User? User { get; set; }

        [ForeignKey("CardID")]
        public Card? Card { get; set; }
    }
}