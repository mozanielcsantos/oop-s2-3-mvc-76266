using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Entities
{
    public class CourseEnrolment
    {
        public int Id { get; set; }

        [Required]
        public int StudentProfileId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DateTime EnrolDate { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = string.Empty;

        public StudentProfile? StudentProfile { get; set; }
        public Course? Course { get; set; }

        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}
