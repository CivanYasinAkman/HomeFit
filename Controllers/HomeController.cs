using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HomeFit.Data;
using HomeFit.Models;

namespace HomeFit.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Dashboard");
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await _context.Users
                .Include(u => u.UserProgram)
                    .ThenInclude(up => up!.Program)
                        .ThenInclude(p => p!.ProgramExercises)
                            .ThenInclude(pe => pe.Exercise)
                .Include(u => u.WorkoutSessions)
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();

            if (!user.OnboardingCompleted)
                return RedirectToAction("Index", "Onboarding");

            return View(user);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel
        {
            RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}