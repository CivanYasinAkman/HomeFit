using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HomeFit.Data;
using HomeFit.Models;

namespace HomeFit.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/exercises
        [HttpGet("exercises")]
        public async Task<IActionResult> GetExercises(
            string? muscle, string? difficulty, bool? isPremium)
        {
            var query = _context.Exercises.AsQueryable();

            if (!string.IsNullOrEmpty(muscle))
                query = query.Where(e => e.MuscleGroup == muscle);

            if (!string.IsNullOrEmpty(difficulty))
                query = query.Where(e => e.Difficulty == difficulty);

            if (isPremium.HasValue)
                query = query.Where(e => e.IsPremium == isPremium.Value);

            var exercises = await query
                .OrderBy(e => e.Name)
                .Select(e => new {
                    e.ExerciseId,
                    e.Name,
                    e.MuscleGroup,
                    e.Difficulty,
                    e.Equipment,
                    e.GifUrl,
                    e.Description,
                    e.IsPremium
                })
                .ToListAsync();

            return Ok(exercises);
        }

        // GET /api/exercises/{id}
        [HttpGet("exercises/{id}")]
        public async Task<IActionResult> GetExercise(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null)
                return NotFound(new { message = "Egzersiz bulunamadı." });
            return Ok(exercise);
        }

        // GET /api/programs
        [HttpGet("programs")]
        public async Task<IActionResult> GetPrograms()
        {
            var programs = await _context.WorkoutPrograms
                .Select(p => new {
                    p.ProgramId,
                    p.Name,
                    p.Description,
                    p.DifficultyLevel,
                    p.Goal,
                    p.DurationWeeks,
                    p.IsPremium
                })
                .ToListAsync();

            return Ok(programs);
        }

        // GET /api/me/program — Giriş yapmış kullanıcının programı
        [HttpGet("me/program")]
        [Authorize]
        public async Task<IActionResult> GetMyProgram()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var userProgram = await _context.UserPrograms
                .Include(up => up.Program)
                    .ThenInclude(p => p!.ProgramExercises)
                        .ThenInclude(pe => pe.Exercise)
                .FirstOrDefaultAsync(up => up.UserId == userId && up.IsActive);

            if (userProgram == null)
                return NotFound(new { message = "Aktif program bulunamadı." });

            return Ok(new {
                programId = userProgram.Program!.ProgramId,
                name = userProgram.Program.Name,
                description = userProgram.Program.Description,
                goal = userProgram.Program.Goal,
                difficultyLevel = userProgram.Program.DifficultyLevel,
                durationWeeks = userProgram.Program.DurationWeeks,
                isPremium = userProgram.Program.IsPremium,
                startDate = userProgram.StartDate,
                exercises = userProgram.Program.ProgramExercises.Select(pe => new {
                    pe.ExerciseId,
                    name = pe.Exercise!.Name,
                    muscleGroup = pe.Exercise.MuscleGroup,
                    gifUrl = pe.Exercise.GifUrl,
                    pe.DayOfWeek,
                    pe.Sets,
                    pe.Reps,
                    pe.OrderInDay
                })
            });
        }

        // GET /api/me/sessions — Seans geçmişi
        [HttpGet("me/sessions")]
        [Authorize]
        public async Task<IActionResult> GetMySessions()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var sessions = await _context.WorkoutSessions
                .Where(ws => ws.UserId == userId)
                .OrderByDescending(ws => ws.StartDate)
                .Take(20)
                .Select(ws => new {
                    ws.SessionId,
                    ws.StartDate,
                    ws.DurationMinutes,
                    ws.Status
                })
                .ToListAsync();

            return Ok(sessions);
        }

        // GET /api/me/progress — Kilo geçmişi
        [HttpGet("me/progress")]
        [Authorize]
        public async Task<IActionResult> GetMyProgress()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var logs = await _context.ProgressLogs
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.LogDate)
                .Select(p => new {
                    p.ProgressId,
                    p.Weight,
                    p.BodyFatPercentage,
                    p.LogDate
                })
                .ToListAsync();

            return Ok(logs);
        }

        // POST /api/onboarding — Rule-based program atama (JWT ile)
        [HttpPost("onboarding")]
        [Authorize]
        public async Task<IActionResult> Onboarding([FromBody] OnboardingRequest req)
        {
            if (req == null)
                return BadRequest(new { message = "Geçersiz veri." });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.Age = req.Age;
            user.Weight = req.Weight;
            user.Height = req.Height;
            user.Gender = req.Gender;
            user.FitnessLevel = req.FitnessLevel;
            user.Goal = req.Goal;
            user.Equipment = req.Equipment;
            user.OnboardingCompleted = true;

            var query = _context.WorkoutPrograms.AsQueryable();
            if (user.MembershipTier != "Premium")
                query = query.Where(p => !p.IsPremium);
            if (!string.IsNullOrEmpty(req.Goal))
                query = query.Where(p => p.Goal == req.Goal);
            if (!string.IsNullOrEmpty(req.FitnessLevel))
                query = query.Where(p => p.DifficultyLevel == req.FitnessLevel);

            var program = await query.FirstOrDefaultAsync()
                ?? await _context.WorkoutPrograms.Where(p => !p.IsPremium).FirstOrDefaultAsync();

            if (program != null)
            {
                var existing = await _context.UserPrograms
                    .FirstOrDefaultAsync(up => up.UserId == userId && up.IsActive);
                if (existing != null) existing.IsActive = false;

                _context.UserPrograms.Add(new UserProgram
                {
                    UserId = userId,
                    ProgramId = program.ProgramId,
                    StartDate = DateTime.UtcNow,
                    IsActive = true
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new {
                message = "Program atandı.",
                assignedProgram = program?.Name ?? "General Fitness",
                goal = req.Goal,
                fitnessLevel = req.FitnessLevel
            });
        }

        // GET /api/stats — Admin istatistikleri
        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStats()
        {
            return Ok(new {
                totalUsers = await _context.Users.CountAsync(),
                premiumUsers = await _context.Users.CountAsync(u => u.MembershipTier == "Premium"),
                totalExercises = await _context.Exercises.CountAsync(),
                freeExercises = await _context.Exercises.CountAsync(e => !e.IsPremium),
                premiumExercises = await _context.Exercises.CountAsync(e => e.IsPremium),
                totalSessions = await _context.WorkoutSessions.CountAsync(),
                totalPrograms = await _context.WorkoutPrograms.CountAsync()
            });
        }
    }

    // Request modeli
    public class OnboardingRequest
    {
        public int Age { get; set; }
        public float Weight { get; set; }
        public float Height { get; set; }
        public string? Gender { get; set; }
        public string? FitnessLevel { get; set; }
        public string? Goal { get; set; }
        public string? Equipment { get; set; }
    }
}