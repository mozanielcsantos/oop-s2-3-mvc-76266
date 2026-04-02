using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Entities;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int enrolmentId)
        {
            var enrolment = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Include(e => e.Course)
                .ThenInclude(c => c!.Branch)
                .FirstOrDefaultAsync(e => e.Id == enrolmentId);

            if (enrolment == null)
            {
                return NotFound();
            }

            var records = await _context.AttendanceRecords
                .Where(a => a.CourseEnrolmentId == enrolmentId)
                .OrderByDescending(a => a.SessionDate)
                .ToListAsync();

            ViewBag.Enrolment = enrolment;

            return View(records);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int enrolmentId)
        {
            var enrolment = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Include(e => e.Course)
                .ThenInclude(c => c!.Branch)
                .FirstOrDefaultAsync(e => e.Id == enrolmentId);

            if (enrolment == null)
            {
                return NotFound();
            }

            ViewBag.Enrolment = enrolment;

            return View(new AttendanceRecord
            {
                CourseEnrolmentId = enrolmentId,
                SessionDate = DateTime.Today,
                Present = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttendanceRecord record)
        {
            var enrolment = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Include(e => e.Course)
                .ThenInclude(c => c!.Branch)
                .FirstOrDefaultAsync(e => e.Id == record.CourseEnrolmentId);

            if (enrolment == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Enrolment = enrolment;
                return View(record);
            }

            _context.AttendanceRecords.Add(record);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { enrolmentId = record.CourseEnrolmentId });
        }
    }
}