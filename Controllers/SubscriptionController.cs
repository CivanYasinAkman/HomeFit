using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HomeFit.Data;
using HomeFit.Models;
using Microsoft.AspNetCore.Authentication;
namespace HomeFit.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        private readonly AppDbContext _context;

        public SubscriptionController(AppDbContext context)
        {
            _context = context;
        }

        // GET /Subscription/Upgrade
        public async Task<IActionResult> Upgrade()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound();

            if (user.MembershipTier == "Premium")
                return RedirectToAction("Dashboard", "Home");

            return View(user);
        }

        // POST /Subscription/Confirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();

            // Simüle ödeme — gerçek gateway yok (FR-ST-33)
            user.MembershipTier = "Premium";

            if (user.Subscription == null)
            {
                _context.Subscriptions.Add(new Subscription
                {
                    UserId = userId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1),
                    PaymentStatus = "Simulated",
                    Type = "Premium"
                });
            }
            else
            {
                user.Subscription.StartDate = DateTime.UtcNow;
                user.Subscription.EndDate = DateTime.UtcNow.AddMonths(1);
                user.Subscription.PaymentStatus = "Simulated";
            }

            await _context.SaveChangesAsync();

            // Cookie claim'ini güncellemek için yeniden giriş yaptır
            await HttpContext.SignOutAsync("CookieAuth");

            var claims = new List<System.Security.Claims.Claim>
            {
                new(System.Security.Claims.ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(System.Security.Claims.ClaimTypes.Email, user.Email),
                new(System.Security.Claims.ClaimTypes.Name, user.Name),
                new(System.Security.Claims.ClaimTypes.Role, user.Role),
                new("MembershipTier", "Premium")
            };
            var identity = new System.Security.Claims.ClaimsIdentity(claims, "CookieAuth");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("CookieAuth", principal,
                new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(24)
                });

            TempData["Success"] = "Premium üyeliğiniz aktif edildi! 🎉";
            return RedirectToAction("Dashboard", "Home");
        }

        // GET /Subscription/Cancel
        public async Task<IActionResult> Cancel()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();
            return View(user);
        }

        // POST /Subscription/CancelConfirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirm()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();

            user.MembershipTier = "Free";
            if (user.Subscription != null)
                user.Subscription.PaymentStatus = "Cancelled";

            await _context.SaveChangesAsync();

            await HttpContext.SignOutAsync("CookieAuth");

            var claims = new List<System.Security.Claims.Claim>
            {
                new(System.Security.Claims.ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(System.Security.Claims.ClaimTypes.Email, user.Email),
                new(System.Security.Claims.ClaimTypes.Name, user.Name),
                new(System.Security.Claims.ClaimTypes.Role, user.Role),
                new("MembershipTier", "Free")
            };
            var identity = new System.Security.Claims.ClaimsIdentity(claims, "CookieAuth");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("CookieAuth", principal,
                new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(24)
                });

            TempData["Success"] = "Premium üyeliğiniz iptal edildi.";
            return RedirectToAction("Dashboard", "Home");
        }
    }
}