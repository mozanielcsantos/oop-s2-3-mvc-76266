using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Entities
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(0, 1000)]
        public decimal MaxScore { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public Course? Course { get; set; }

        public ICollection<AssignmentResult> Results { get; set; } = new List<AssignmentResult>();
    }
}
