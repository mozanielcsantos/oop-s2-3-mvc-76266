using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Entities
{
    public class Exam
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(0, 1000)]
        public decimal MaxScore { get; set; }

        [Required]
        public bool ResultsReleased { get; set; }

        public Course? Course { get; set; }

        public ICollection<ExamResult> Results { get; set; } = new List<ExamResult>();
    }
}
