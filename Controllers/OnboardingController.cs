using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HomeFit.Data;
using HomeFit.Models;

namespace HomeFit.Controllers
{
    [Authorize]
    public class OnboardingController : Controller
    {
        private readonly AppDbContext _context;

        public OnboardingController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(User formData)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            // Fiziksel metrikleri güncelle
            user.Age = formData.Age;
            user.Weight = formData.Weight;
            user.Height = formData.Height;
            user.Gender = formData.Gender;
            user.FitnessLevel = formData.FitnessLevel;
            user.Goal = formData.Goal;
            user.Equipment = formData.Equipment;
            user.OnboardingCompleted = true;

            // Mevcut aktif programı pasife çek
            var existing = await _context.UserPrograms
                .FirstOrDefaultAsync(up => up.UserId == userId && up.IsActive);
            if (existing != null)
            {
                existing.IsActive = false;
            }

            // Kural tabanlı program ata
            var program = await AssignProgram(user);
            if (program != null)
            {
                _context.UserPrograms.Add(new UserProgram
                {
                    UserId = userId,
                    ProgramId = program.ProgramId,
                    StartDate = DateTime.UtcNow,
                    IsActive = true
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard", "Home");
        }

        private async Task<WorkoutProgram?> AssignProgram(User user)
        {
            var query = _context.WorkoutPrograms.AsQueryable();

            // Premium kontrolü
            if (user.MembershipTier != "Premium")
                query = query.Where(p => !p.IsPremium);

            // Goal eşleştir
            if (!string.IsNullOrEmpty(user.Goal))
                query = query.Where(p => p.Goal == user.Goal);

            // Fitness seviyesi eşleştir
            if (!string.IsNullOrEmpty(user.FitnessLevel))
                query = query.Where(p => p.DifficultyLevel == user.FitnessLevel);

            var match = await query.FirstOrDefaultAsync();

            // Eşleşme yoksa genel programı ver
            if (match == null)
                match = await _context.WorkoutPrograms
                    .Where(p => !p.IsPremium)
                    .FirstOrDefaultAsync();

            return match;
        }
    }
}