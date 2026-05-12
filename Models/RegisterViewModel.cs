using System.ComponentModel.DataAnnotations;

namespace HomeFit.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email girin.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Güvenlik sorusu zorunludur.")]
        public string SecurityQuestion { get; set; } = "";

        [Required(ErrorMessage = "Güvenlik cevabı zorunludur.")]
        public string SecurityAnswer { get; set; } = "";

        [Required(ErrorMessage = "KVKK onayı zorunludur.")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Gizlilik politikasını kabul etmelisiniz.")]
        public bool AcceptPrivacyPolicy { get; set; }
    }
}