using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.ViewModels;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FacultyController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
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

            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);

            if (faculty == null)
            {
                return NotFound();
            }

            var courses = await _context.FacultyCourseAssignments
                .Where(x => x.FacultyProfileId == faculty.Id)
                .Include(x => x.Course)
                .ThenInclude(c => c!.Branch)
                .Select(x => new FacultyCourseItemViewModel
                {
                    CourseName = x.Course!.Name,
                    BranchName = x.Course.Branch!.Name,
                    StudentCount = x.Course.Enrolments.Count
                })
                .ToListAsync();

            var vm = new FacultyDashboardViewModel
            {
                FacultyName = faculty.Name,
                Courses = courses
            };

            return View(vm);
        }
    }
}