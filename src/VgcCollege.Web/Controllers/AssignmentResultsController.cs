using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Entities;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class AssignmentResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var results = await _context.AssignmentResults
                .Include(r => r.Assignment)
                .ThenInclude(a => a!.Course)
                .ThenInclude(c => c!.Branch)
                .Include(r => r.StudentProfile)
                .OrderBy(r => r.Assignment!.Title)
                .ThenBy(r => r.StudentProfile!.Name)
                .ToListAsync();

            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();

            return View(new AssignmentResult
            {
                Score = 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssignmentResult result)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(result.AssignmentId, result.StudentProfileId);
                return View(result);
            }

            var exists = await _context.AssignmentResults
                .AnyAsync(r => r.AssignmentId == result.AssignmentId && r.StudentProfileId == result.StudentProfileId);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "This student already has a result for this assignment.");
                await LoadDropdownsAsync(result.AssignmentId, result.StudentProfileId);
                return View(result);
            }

            _context.AssignmentResults.Add(result);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync(int? selectedAssignmentId = null, int? selectedStudentId = null)
        {
            var assignments = await _context.Assignments
                .Include(a => a.Course)
                .ThenInclude(c => c!.Branch)
                .OrderBy(a => a.Title)
                .ToListAsync();

            var students = await _context.StudentProfiles
                .OrderBy(s => s.Name)
                .ToListAsync();

            ViewBag.Assignments = new SelectList(
                assignments.Select(a => new
                {
                    a.Id,
                    DisplayName = $"{a.Title} - {a.Course!.Name}"
                }),
                "Id",
                "DisplayName",
                selectedAssignmentId);

            ViewBag.Students = new SelectList(
                students,
                "Id",
                "Name",
                selectedStudentId);
        }
    }
}
