using Microsoft.AspNetCore.Mvc;
using HomeFit.Models;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> GetExercises()
        {
            var exercises = await _context.Exercises.ToListAsync();
            return Ok(exercises);
        }

        // GET /api/exercises/{id}
        [HttpGet("exercises/{id}")]
        public async Task<IActionResult> GetExercise(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null)
                return NotFound(new { message = "Exercise not found" });
            return Ok(exercise);
        }

        // POST /api/onboarding
        [HttpPost("onboarding")]
        public IActionResult Onboarding([FromBody] User user)
        {
            if (user == null)
                return BadRequest(new { message = "Invalid user data" });

            string program = AssignProgram(user);
            return Ok(new
            {
                userName = user.Name,
                goal = user.Goal,
                fitnessLevel = user.FitnessLevel,
                assignedProgram = program
            });
        }

        private string AssignProgram(User user)
        {
            if (user.Goal == "fat_loss" && user.FitnessLevel == "beginner")
                return "Beginner Fat Loss Program — 3 days/week, bodyweight cardio";
            else if (user.Goal == "muscle_gain" && user.FitnessLevel == "beginner")
                return "Beginner Muscle Gain Program — 3 days/week, basic strength";
            else if (user.Goal == "muscle_gain" && user.FitnessLevel == "intermediate")
                return "Intermediate Strength Program — 4 days/week, progressive overload";
            else if (user.Goal == "fat_loss" && user.FitnessLevel == "intermediate")
                return "Intermediate Fat Loss Program — 4 days/week, HIIT + strength";
            else
                return "General Fitness Program — 3 days/week, full body";
        }
    }
}