namespace HomeFit.Models
{
    public class SessionExercise
    {
        public int SessionExerciseId { get; set; }
        public int SessionId { get; set; }
        public int ExerciseId { get; set; }
        public int SetsCompleted { get; set; }
        public int RepsCompleted { get; set; }
        public float? WeightUsed { get; set; }

        // Navigation
        public WorkoutSession? Session { get; set; }
        public Exercise? Exercise { get; set; }
    }
}