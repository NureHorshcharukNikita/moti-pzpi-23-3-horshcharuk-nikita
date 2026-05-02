using System.ComponentModel.DataAnnotations;

namespace Lab1.Models
{
    public class Effect
    {
        [Key]
        public int EffectID { get; set; }

        [Required]
        public string EffectCode { get; set; } = "";

        [Required]
        public string EffectName { get; set; } = "";

        [Required]
        public string EffectDescription { get; set; } = "";

        public ICollection<CardEffect>? CardEffects { get; set; }
    }
}