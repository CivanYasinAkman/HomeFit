using System.ComponentModel.DataAnnotations; 

namespace HomeFit.Models
{
    public class WorkoutSession
    {
        [Key] 
        public int SessionId { get; set; }
        
        public int UserId { get; set; }
        public int? ProgramId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = "Completed";

        public User? User { get; set; }
        public ICollection<SessionExercise> SessionExercises { get; set; } = new List<SessionExercise>();
    }
}