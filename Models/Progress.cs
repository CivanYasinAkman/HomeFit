namespace HomeFit.Models
{
    public class Progress
    {
        public int ProgressId { get; set; }
        public int UserId { get; set; }
        public float Weight { get; set; }
        public float? BodyFatPercentage { get; set; }
        public DateTime LogDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
    }
}