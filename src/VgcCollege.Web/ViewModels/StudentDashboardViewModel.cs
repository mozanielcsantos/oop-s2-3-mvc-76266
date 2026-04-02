namespace VgcCollege.Web.ViewModels
{
    public class StudentDashboardViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<StudentCourseItemViewModel> Courses { get; set; } = new();
        public List<StudentAssignmentItemViewModel> AssignmentResults { get; set; } = new();
        public List<StudentExamItemViewModel> ReleasedExamResults { get; set; } = new();
    }
}