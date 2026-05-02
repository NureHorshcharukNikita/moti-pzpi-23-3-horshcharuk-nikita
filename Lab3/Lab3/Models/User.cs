using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab3.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } = "User";

        public int? EvaluatorID { get; set; }

        [ForeignKey("EvaluatorID")]
        public Evaluator? Evaluator { get; set; }

        public ICollection<CardRanking>? CardRankings { get; set; }
    }
}
