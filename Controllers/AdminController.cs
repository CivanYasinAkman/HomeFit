using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HomeFit.Data;
using HomeFit.Models;

namespace HomeFit.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET /Admin
        public async Task<IActionResult> Index()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalExercises = await _context.Exercises.CountAsync();
            var premiumUsers = await _context.Users.CountAsync(u => u.MembershipTier == "Premium");
            var totalSessions = await _context.WorkoutSessions.CountAsync();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalExercises = totalExercises;
            ViewBag.PremiumUsers = premiumUsers;
            ViewBag.TotalSessions = totalSessions;

            return View();
        }

        // GET /Admin/Users
        public async Task<IActionResult> Users(string? search, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u => u.Name.Contains(search) || u.Email.Contains(search));

            var total = await query.CountAsync();
            var users = await query
                .OrderByDescending(u => u.JoinDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.TotalCount = total;

            return View(users);
        }

        // POST /Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"{user.Name} adlı kullanıcı silindi.";
            }
            return RedirectToAction("Users");
        }

        // GET /Admin/Exercises
        public async Task<IActionResult> Exercises(string? search)
        {
            var query = _context.Exercises.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.Name!.Contains(search) || e.MuscleGroup!.Contains(search));

            var exercises = await query.OrderBy(e => e.Name).ToListAsync();
            ViewBag.Search = search;
            return View(exercises);
        }

        // GET /Admin/CreateExercise
        public IActionResult CreateExercise() => View(new Exercise());

        // POST /Admin/CreateExercise
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExercise(Exercise model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Exercises.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"{model.Name} egzersizi eklendi.";
            return RedirectToAction("Exercises");
        }

        // GET /Admin/EditExercise/5
        public async Task<IActionResult> EditExercise(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null) return NotFound();
            return View(exercise);
        }

        // POST /Admin/EditExercise/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExercise(Exercise model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Exercises.Update(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"{model.Name} egzersizi güncellendi.";
            return RedirectToAction("Exercises");
        }

        // POST /Admin/DeleteExercise
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExercise(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise != null)
            {
                _context.Exercises.Remove(exercise);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"{exercise.Name} egzersizi silindi.";
            }
            return RedirectToAction("Exercises");
        }
    }
}