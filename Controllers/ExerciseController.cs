using Microsoft.AspNetCore.Mvc;
using HomeFit.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeFit.Controllers
{
    public class ExerciseController : Controller
    {
        private readonly AppDbContext _context;

        public ExerciseController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var exercises = await _context.Exercises.ToListAsync();
            return View(exercises);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null) return NotFound();
            return View(exercise);
        }
    }
}