using System.ComponentModel.DataAnnotations;

namespace HomeFit.Models
{
    public class Exercise
    {
        public int ExerciseId { get; set; }

        [Required(ErrorMessage = "Egzersiz adı zorunludur.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Kas grubu zorunludur.")]
        public string? MuscleGroup { get; set; }

        [Required(ErrorMessage = "Zorluk seviyesi zorunludur.")]
        public string? Difficulty { get; set; }

        public string? Equipment { get; set; } = "None";

        public string? GifUrl { get; set; }

        public string? Description { get; set; }

        public bool IsPremium { get; set; } = false;

        // Navigation
        public ICollection<ProgramExercise> ProgramExercises { get; set; }
            = new List<ProgramExercise>();
        public ICollection<SessionExercise> SessionExercises { get; set; }
            = new List<SessionExercise>();
    }
}