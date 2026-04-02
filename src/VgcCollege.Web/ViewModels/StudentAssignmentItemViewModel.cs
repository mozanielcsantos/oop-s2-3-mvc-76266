namespace VgcCollege.Web.ViewModels
{
    public class StudentAssignmentItemViewModel
    {
        public string AssignmentTitle { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public string? Feedback { get; set; }
    }
}