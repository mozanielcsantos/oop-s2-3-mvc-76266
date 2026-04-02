using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Entities
{
    public class FacultyCourseAssignment
    {
        public int Id { get; set; }

        [Required]
        public int FacultyProfileId { get; set; }

        [Required]
        public int CourseId { get; set; }

        public FacultyProfile? FacultyProfile { get; set; }
        public Course? Course { get; set; }
    }
}
