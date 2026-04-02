using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Entities;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class ExamResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var results = await _context.ExamResults
                .Include(r => r.Exam)
                .ThenInclude(e => e!.Course)
                .ThenInclude(c => c!.Branch)
                .Include(r => r.StudentProfile)
                .OrderBy(r => r.Exam!.Title)
                .ThenBy(r => r.StudentProfile!.Name)
                .ToListAsync();

            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var exams = await _context.Exams
                .Include(e => e.Course)
                .ThenInclude(c => c!.Branch)
                .OrderBy(e => e.Title)
                .ToListAsync();

            var students = await _context.StudentProfiles
                .OrderBy(s => s.Name)
                .ToListAsync();

            ViewBag.Exams = new SelectList(
                exams.Select(e => new
                {
                    e.Id,
                    DisplayName = $"{e.Title} - {e.Course!.Name}"
                }),
                "Id",
                "DisplayName");

            ViewBag.Students = new SelectList(
                students,
                "Id",
                "Name");

            return View(new ExamResult
            {
                Score = 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamResult result)
        {
            var exams = await _context.Exams
                .Include(e => e.Course)
                .ThenInclude(c => c!.Branch)
                .OrderBy(e => e.Title)
                .ToListAsync();

            var students = await _context.StudentProfiles
                .OrderBy(s => s.Name)
                .ToListAsync();

            ViewBag.Exams = new SelectList(
                exams.Select(e => new
                {
                    e.Id,
                    DisplayName = $"{e.Title} - {e.Course!.Name}"
                }),
                "Id",
                "DisplayName",
                result.ExamId);

            ViewBag.Students = new SelectList(
                students,
                "Id",
                "Name",
                result.StudentProfileId);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            var exists = await _context.ExamResults
                .AnyAsync(r => r.ExamId == result.ExamId && r.StudentProfileId == result.StudentProfileId);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "This student already has a result for this exam.");
                return View(result);
            }

            _context.ExamResults.Add(result);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
