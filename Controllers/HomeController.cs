using Microsoft.AspNetCore.Mvc;
using HomeFit.Models;
using HomeFit.Data;

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
            return View();
        }
    }
}