namespace HomeFit.Models
{
    public class UserProgram
    {
        public int UserProgramId { get; set; }
        public int UserId { get; set; }
        public int ProgramId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation
        public User? User { get; set; }
        public WorkoutProgram? Program { get; set; }
    }
}