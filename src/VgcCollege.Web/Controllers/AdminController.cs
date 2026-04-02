using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.ViewModels;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalBranches = await _context.Branches.CountAsync(),
                TotalCourses = await _context.Courses.CountAsync(),
                TotalStudents = await _context.StudentProfiles.CountAsync(),
                TotalFaculty = await _context.FacultyProfiles.CountAsync(),
                TotalEnrolments = await _context.CourseEnrolments.CountAsync(),
                TotalExams = await _context.Exams.CountAsync()
            };

            return View(vm);
        }
    }
}
