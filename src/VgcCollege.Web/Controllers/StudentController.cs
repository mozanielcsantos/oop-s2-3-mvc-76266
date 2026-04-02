using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.ViewModels;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var student = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.IdentityUserId == user.Id);

            if (student == null)
            {
                return NotFound();
            }

            var courses = await _context.CourseEnrolments
                .Where(e => e.StudentProfileId == student.Id)
                .Include(e => e.Course)
                .ThenInclude(c => c!.Branch)
                .Select(e => new StudentCourseItemViewModel
                {
                    CourseName = e.Course!.Name,
                    BranchName = e.Course.Branch!.Name,
                    Status = e.Status,
                    EnrolDate = e.EnrolDate
                })
                .ToListAsync();

            var assignmentResults = await _context.AssignmentResults
                .Where(r => r.StudentProfileId == student.Id)
                .Include(r => r.Assignment)
                .Select(r => new StudentAssignmentItemViewModel
                {
                    AssignmentTitle = r.Assignment!.Title,
                    Score = r.Score,
                    Feedback = r.Feedback
                })
                .ToListAsync();

            var releasedExamResults = await _context.ExamResults
                .Where(r => r.StudentProfileId == student.Id)
                .Include(r => r.Exam)
                .Where(r => r.Exam != null && r.Exam.ResultsReleased)
                .Select(r => new StudentExamItemViewModel
                {
                    ExamTitle = r.Exam!.Title,
                    Score = r.Score,
                    Grade = r.Grade
                })
                .ToListAsync();

            var vm = new StudentDashboardViewModel
            {
                StudentName = student.Name,
                StudentNumber = student.StudentNumber,
                Email = student.Email,
                Courses = courses,
                AssignmentResults = assignmentResults,
                ReleasedExamResults = releasedExamResults
            };

            return View(vm);
        }
    }
}