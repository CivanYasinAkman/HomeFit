using System.ComponentModel.DataAnnotations;

namespace HomeFit.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string PasswordHash { get; set; } = "";

        [Required]
        public string SecurityQuestion { get; set; } = "";

        [Required]
        public string SecurityAnswer { get; set; } = "";

        public string Role { get; set; } = "User"; // "User" veya "Admin"

        public string MembershipTier { get; set; } = "Free"; // "Free" veya "Premium"

        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        public string? AvatarUrl { get; set; }

        // Profil / fiziksel metrikler
        public int Age { get; set; }
        public float Weight { get; set; }
        public float Height { get; set; }
        public string? Gender { get; set; }
        public string? FitnessLevel { get; set; }
        public string? Goal { get; set; }
        public string? Equipment { get; set; }
        public bool OnboardingCompleted { get; set; } = false;

        // Navigation properties
        public ICollection<WorkoutSession> WorkoutSessions { get; set; } = new List<WorkoutSession>();
        public ICollection<Progress> ProgressLogs { get; set; } = new List<Progress>();
        public Subscription? Subscription { get; set; }
        public UserProgram? UserProgram { get; set; }
    }
}