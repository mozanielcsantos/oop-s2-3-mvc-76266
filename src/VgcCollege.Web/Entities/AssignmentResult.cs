using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Entities
{
    public class AssignmentResult
    {
        public int Id { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        [Required]
        public int StudentProfileId { get; set; }

        [Required]
        [Range(0, 1000)]
        public decimal Score { get; set; }

        [StringLength(500)]
        public string? Feedback { get; set; }

        public Assignment? Assignment { get; set; }
        public StudentProfile? StudentProfile { get; set; }
    }
}
