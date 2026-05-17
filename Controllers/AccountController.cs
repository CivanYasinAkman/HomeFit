using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HomeFit.Data;
using HomeFit.Models;
using HomeFit.Services;
using Microsoft.EntityFrameworkCore;

namespace HomeFit.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AccountController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // GET /Account/Register
        public IActionResult Register() => View();

        // POST /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Email benzersizlik kontrolü (FR-ST-02)
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Bu email adresi zaten kayıtlı.");
                return View(model);
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password, workFactor: 10),
                SecurityQuestion = model.SecurityQuestion,
                SecurityAnswer = model.SecurityAnswer.ToLower().Trim(),
                Role = "User",
                MembershipTier = "Free",
                JoinDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Kayıt sonrası otomatik giriş
            await SignInUser(user);
            return RedirectToAction("Index", "Onboarding");
        }

        // GET /Account/Login
        public IActionResult Login() => View();

        // POST /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Email veya şifre hatalı.");
                return View(model);
            }

            await SignInUser(user);

            if (!user.OnboardingCompleted)
                return RedirectToAction("Index", "Onboarding");

            return RedirectToAction("Dashboard", "Home");
        }

        // GET /Account/Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Home");
        }

        // GET /Account/ForgotPassword
        public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

        // POST /Account/ForgotPassword — email ile soru yükle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoadQuestion(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var model = new ForgotPasswordViewModel { Email = email };

            if (user == null)
            {
                ModelState.AddModelError("", "Bu email ile kayıtlı kullanıcı bulunamadı.");
                return View("ForgotPassword", model);
            }

            model.SecurityQuestion = user.SecurityQuestion;
            model.QuestionLoaded = true;
            return View("ForgotPassword", model);
        }

        // POST /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ForgotPasswordViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                model.QuestionLoaded = true;
                return View("ForgotPassword", model);
            }

            if (user.SecurityAnswer != model.SecurityAnswer.ToLower().Trim())
            {
                ModelState.AddModelError("SecurityAnswer", "Güvenlik cevabı yanlış.");
                model.SecurityQuestion = user.SecurityQuestion;
                model.QuestionLoaded = true;
                return View("ForgotPassword", model);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, workFactor: 10);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Şifreniz başarıyla sıfırlandı. Giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        // GET /Account/AccessDenied
        public IActionResult AccessDenied() => View();

        // GET /Account/Profile
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users
                .Include(u => u.UserProgram)
                    .ThenInclude(up => up!.Program)
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();
            return View(user);
        }

        // POST /Account/DeleteAccount
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Home");
        }
// GET /Account/EditProfile
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST /Account/EditProfile
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(User formData)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.Name = formData.Name;
            user.Age = formData.Age;
            user.Weight = formData.Weight;
            user.Height = formData.Height;
            user.Gender = formData.Gender;

            await _context.SaveChangesAsync();

            // Cookie'deki ismi güncelle
            await HttpContext.SignOutAsync("CookieAuth");
            await SignInUser(user);

            TempData["Success"] = "Profil bilgileri güncellendi.";
            return RedirectToAction("Profile");
        }

        // POST /Account/ChangePassword
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            string currentPassword, string newPassword)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                TempData["Error"] = "Mevcut şifre hatalı.";
                return RedirectToAction("Profile");
            }

            if (newPassword.Length < 6)
            {
                TempData["Error"] = "Yeni şifre en az 6 karakter olmalıdır.";
                return RedirectToAction("Profile");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                newPassword, workFactor: 10);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Şifreniz başarıyla güncellendi.";
            return RedirectToAction("Profile");
        }
        
        // Yardımcı: cookie ile oturum aç
        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("MembershipTier", user.MembershipTier)
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(24)
                });
        }
    }
}