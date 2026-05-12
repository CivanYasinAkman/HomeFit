namespace HomeFit.Models
{
    public class ProgramExercise
    {
        public int ProgramExerciseId { get; set; }
        public int ProgramId { get; set; }
        public int ExerciseId { get; set; }
        public string? DayOfWeek { get; set; } // "Monday", "Wednesday" vb.
        public int OrderInDay { get; set; }
        public int Sets { get; set; } = 3;
        public int Reps { get; set; } = 10;

        // Navigation
        public WorkoutProgram? Program { get; set; }
        public Exercise? Exercise { get; set; }
    }
}