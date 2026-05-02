using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab1.Models
{
    public class CardEffect
    {
        [Key]
        public int CardEffectID { get; set; }

        [Required]
        public int CardID { get; set; }

        [Required]
        public int EffectID { get; set; }

        [Required]
        public double Value { get; set; }

        [ForeignKey("CardID")]
        public Card? Card { get; set; }

        [ForeignKey("EffectID")]
        public Effect? Effect { get; set; }
    }
}