using System.ComponentModel.DataAnnotations;

namespace HomeFit.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Şifre zorunludur.")]
        public string Password { get; set; } = "";
    }
}