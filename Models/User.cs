namespace HomeFit.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int Age { get; set; }
        public float Weight { get; set; }
        public float Height { get; set; }
        public string? Gender { get; set; }
        public string? FitnessLevel { get; set; }
        public string? Goal { get; set; }
        public string? Equipment { get; set; }
        public bool IsPremium { get; set; }
    }
}