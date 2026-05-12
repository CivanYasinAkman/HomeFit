using System.ComponentModel.DataAnnotations;

namespace HomeFit.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        public string? SecurityQuestion { get; set; }
        public bool QuestionLoaded { get; set; } = false;

        [Required(ErrorMessage = "Cevap zorunludur.")]
        public string SecurityAnswer { get; set; } = "";

        [Required(ErrorMessage = "Yeni şifre zorunludur.")]
        [MinLength(6)]
        public string NewPassword { get; set; } = "";
    }
}