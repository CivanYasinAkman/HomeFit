using Microsoft.AspNetCore.Mvc;
using HomeFit.Models;
using HomeFit.Data;

namespace HomeFit.Controllers
{
    public class OnboardingController : Controller
    {
        private readonly AppDbContext _context;

        public OnboardingController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(User user)
        {
            if (!ModelState.IsValid)
                return View("Index", user);

            string program = AssignProgram(user);
            ViewBag.Program = program;
            ViewBag.UserName = user.Name;
            return View("Result");
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