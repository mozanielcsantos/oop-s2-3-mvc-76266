namespace VgcCollege.Web.ViewModels
{
    public class FacultyDashboardViewModel
    {
        public string FacultyName { get; set; } = string.Empty;
        public List<FacultyCourseItemViewModel> Courses { get; set; } = new();
    }
}