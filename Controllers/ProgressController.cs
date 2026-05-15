using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HomeFit.Data;
using HomeFit.Models;

namespace HomeFit.Controllers
{
    [Authorize]
    public class ProgressController : Controller
    {
        private readonly AppDbContext _context;

        public ProgressController(AppDbContext context)
        {
            _context = context;
        }

        // GET /Progress
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var sessions = await _context.WorkoutSessions
                .Where(ws => ws.UserId == userId)
                .OrderByDescending(ws => ws.StartDate)
                .Take(30)
                .ToListAsync();

            var progressLogs = await _context.ProgressLogs
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.LogDate)
                .ToListAsync();

            ViewBag.Sessions = sessions;
            ViewBag.ProgressLogs = progressLogs;

            // Chart verisi için JSON
            var sessionDates = sessions
                .GroupBy(s => s.StartDate.ToString("dd.MM"))
                .Select(g => new { date = g.Key, count = g.Count() })
                .OrderBy(x => x.date)
                .ToList();

            ViewBag.SessionDatesJson = System.Text.Json.JsonSerializer.Serialize(
                sessionDates.Select(x => x.date));
            ViewBag.SessionCountsJson = System.Text.Json.JsonSerializer.Serialize(
                sessionDates.Select(x => x.count));

            var weightDates = progressLogs.Select(p => p.LogDate.ToString("dd.MM")).ToList();
            var weightValues = progressLogs.Select(p => p.Weight).ToList();

            ViewBag.WeightDatesJson = System.Text.Json.JsonSerializer.Serialize(weightDates);
            ViewBag.WeightValuesJson = System.Text.Json.JsonSerializer.Serialize(weightValues);

            return View();
        }

        // POST /Progress/LogWeight
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogWeight(float weight, float? bodyFatPercentage)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            _context.ProgressLogs.Add(new Progress
            {
                UserId = userId,
                Weight = weight,
                BodyFatPercentage = bodyFatPercentage,
                LogDate = DateTime.UtcNow
            });

            // User tablosundaki kiloyu da güncelle
            var user = await _context.Users.FindAsync(userId);
            if (user != null) user.Weight = weight;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Ölçüm kaydedildi.";
            return RedirectToAction("Index");
        }

        // POST /Progress/LogSession
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogSession(int? programId, int durationMinutes)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            _context.WorkoutSessions.Add(new WorkoutSession
            {
                UserId = userId,
                ProgramId = programId,
                StartDate = DateTime.UtcNow,
                DurationMinutes = durationMinutes,
                Status = "Completed"
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = "Antrenman seansı kaydedildi.";
            return RedirectToAction("Index");
        }
    }
}