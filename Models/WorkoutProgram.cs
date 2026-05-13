using System.ComponentModel.DataAnnotations;

namespace HomeFit.Models
{
    public class WorkoutProgram
    {
        [Key]
        public int ProgramId { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string? Description { get; set; }
        public string? DifficultyLevel { get; set; }
        public string? Goal { get; set; }
        public int DurationWeeks { get; set; } = 4;
        public bool IsPremium { get; set; } = false;

        public ICollection<UserProgram> UserPrograms { get; set; } = new List<UserProgram>();
        public ICollection<ProgramExercise> ProgramExercises { get; set; } = new List<ProgramExercise>();
    }
}