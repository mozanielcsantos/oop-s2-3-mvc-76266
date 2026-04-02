using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Entities;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var assignments = await _context.Assignments
                .Include(a => a.Course)
                .ThenInclude(c => c!.Branch)
                .OrderBy(a => a.DueDate)
                .ToListAsync();

            return View(assignments);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var courses = await _context.Courses
                .Include(c => c.Branch)
                .ToListAsync();

            ViewBag.Courses = new SelectList(
                courses.Select(c => new
                {
                    c.Id,
                    Display = $"{c.Name} - {c.Branch!.Name}"
                }),
                "Id",
                "Display");

            return View(new Assignment
            {
                DueDate = DateTime.Today.AddDays(7)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assignment assignment)
        {
            if (!ModelState.IsValid)
            {
                await LoadCourses();
                return View(assignment);
            }

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Branch)
                .ToListAsync();

            ViewBag.Courses = new SelectList(
                courses.Select(c => new
                {
                    c.Id,
                    Display = $"{c.Name} - {c.Branch!.Name}"
                }),
                "Id",
                "Display");
        }
    }
}
