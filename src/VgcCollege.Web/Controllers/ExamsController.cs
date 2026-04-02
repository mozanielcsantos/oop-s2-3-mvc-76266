using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Entities;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin,Faculty")]
    public class ExamsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var exams = await _context.Exams
                .Include(e => e.Course)
                .ThenInclude(c => c!.Branch)
                .ToListAsync();

            return View(exams);
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

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Exam exam)
        {
            if (!ModelState.IsValid)
                return View(exam);

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
