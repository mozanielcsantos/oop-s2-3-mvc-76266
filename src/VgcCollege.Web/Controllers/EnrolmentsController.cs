using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Entities;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class EnrolmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrolmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var enrolments = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Include(e => e.Course)
                .ThenInclude(c => c!.Branch)
                .OrderBy(e => e.StudentProfile!.Name)
                .ThenBy(e => e.Course!.Name)
                .ToListAsync();

            return View(enrolments);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View(new CourseEnrolment
            {
                EnrolDate = DateTime.Today,
                Status = "Active"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseEnrolment enrolment)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(enrolment.StudentProfileId, enrolment.CourseId);
                return View(enrolment);
            }

            var exists = await _context.CourseEnrolments
                .AnyAsync(e => e.StudentProfileId == enrolment.StudentProfileId && e.CourseId == enrolment.CourseId);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "This student is already enrolled in this course.");
                await LoadDropdownsAsync(enrolment.StudentProfileId, enrolment.CourseId);
                return View(enrolment);
            }

            _context.CourseEnrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var enrolment = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Include(e => e.Course)
                .ThenInclude(c => c!.Branch)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrolment == null)
            {
                return NotFound();
            }

            return View(enrolment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrolment = await _context.CourseEnrolments.FindAsync(id);

            if (enrolment == null)
            {
                return NotFound();
            }

            _context.CourseEnrolments.Remove(enrolment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync(int? selectedStudentId = null, int? selectedCourseId = null)
        {
            var students = await _context.StudentProfiles
                .OrderBy(s => s.Name)
                .ToListAsync();

            var courses = await _context.Courses
                .Include(c => c.Branch)
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Students = new SelectList(students, "Id", "Name", selectedStudentId);
            ViewBag.Courses = new SelectList(
                courses.Select(c => new
                {
                    c.Id,
                    DisplayName = $"{c.Name} - {c.Branch!.Name}"
                }),
                "Id",
                "DisplayName",
                selectedCourseId);
        }
    }
}