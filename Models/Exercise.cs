namespace HomeFit.Models
{
    public class Exercise
    {
        public int ExerciseId { get; set; }
        public string? Name { get; set; }
        public string? MuscleGroup { get; set; }
        public string? Difficulty { get; set; }
        public string? Equipment { get; set; }
        public string? GifUrl { get; set; }
        public string? Description { get; set; }
        public bool IsPremium { get; set; }
    }
}