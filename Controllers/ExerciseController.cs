using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HomeFit.Data;
using HomeFit.Models;

namespace HomeFit.Controllers
{
    public class ExerciseController : Controller
    {
        private readonly AppDbContext _context;

        public ExerciseController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? muscle, string? difficulty, string? search)
        {
            var query = _context.Exercises.AsQueryable();

            if (!string.IsNullOrWhiteSpace(muscle))
                query = query.Where(e => e.MuscleGroup == muscle);

            if (!string.IsNullOrWhiteSpace(difficulty))
                query = query.Where(e => e.Difficulty == difficulty);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.Name!.Contains(search));

            var exercises = await query.OrderBy(e => e.Name).ToListAsync();

            ViewBag.MuscleFilter = muscle;
            ViewBag.DifficultyFilter = difficulty;
            ViewBag.Search = search;

            return View(exercises);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null) return NotFound();

            // Feature Gating — Premium egzersiz kontrolü (FR-ST-34)
            if (exercise.IsPremium && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var tier = User.FindFirstValue("MembershipTier");
                if (tier != "Premium")
                    return RedirectToAction("Upgrade", "Subscription");
            }
            else if (exercise.IsPremium && (User.Identity == null || !User.Identity.IsAuthenticated))
            {
                return RedirectToAction("Login", "Account");
            }

            return View(exercise);
        }
    }
}