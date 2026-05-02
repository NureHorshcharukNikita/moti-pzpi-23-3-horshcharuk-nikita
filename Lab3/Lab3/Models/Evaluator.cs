using System.ComponentModel.DataAnnotations;

namespace Lab3.Models
{
    public class Evaluator
    {
        [Key]
        public int EvaluatorID { get; set; }

        [Required]
        public string EvaluatorName { get; set; } = "";

        [Range(0, int.MaxValue)]
        public int CompetenceRank { get; set; }
    }
}