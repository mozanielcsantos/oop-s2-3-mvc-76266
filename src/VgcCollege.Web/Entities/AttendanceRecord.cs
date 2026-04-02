using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Entities
{
    public class AttendanceRecord
    {
        public int Id { get; set; }

        [Required]
        public int CourseEnrolmentId { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }

        [Required]
        public bool Present { get; set; }

        public CourseEnrolment? CourseEnrolment { get; set; }
    }
}
