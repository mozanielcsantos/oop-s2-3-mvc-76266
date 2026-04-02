namespace VgcCollege.Web.Entities
{
    public class ExamResult
    {
        private string? _grade;

        public int Id { get; set; }

        public int ExamId { get; set; }
        public Exam? Exam { get; set; }

        public int StudentProfileId { get; set; }
        public StudentProfile? StudentProfile { get; set; }

        public decimal Score { get; set; }

        public string Grade
        {
            get
            {
                if (Score >= 70) return "A";
                if (Score >= 60) return "B";
                if (Score >= 50) return "C";
                return "Fail";
            }
            set
            {
                _grade = value;
            }
        }
    }
}
