using System.ComponentModel.DataAnnotations;

namespace Lab3.Models
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

        [Required]
        public double Etalon { get; set; }

        [Required]
        public string Type { get; set; } = "MAX";

        public ICollection<CardEffect>? CardEffects { get; set; }
    }
}