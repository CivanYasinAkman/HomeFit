using Microsoft.EntityFrameworkCore;

namespace HomeFit.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise { ExerciseId = 1, Name = "Squat", MuscleGroup = "Legs", Difficulty = "Beginner", Equipment = "None", GifUrl = "https://media.giphy.com/media/squat.gif", Description = "Stand with feet shoulder-width apart, lower your body as if sitting back into a chair.", IsPremium = false },
                new Exercise { ExerciseId = 2, Name = "Push-up", MuscleGroup = "Chest", Difficulty = "Beginner", Equipment = "None", GifUrl = "https://media.giphy.com/media/pushup.gif", Description = "Keep your body straight, lower your chest to the floor and push back up.", IsPremium = false },
                new Exercise { ExerciseId = 3, Name = "Plank", MuscleGroup = "Core", Difficulty = "Beginner", Equipment = "None", GifUrl = "https://media.giphy.com/media/plank.gif", Description = "Hold a push-up position with your body in a straight line.", IsPremium = false },
                new Exercise { ExerciseId = 4, Name = "Lunges", MuscleGroup = "Legs", Difficulty = "Beginner", Equipment = "None", GifUrl = "https://media.giphy.com/media/lunges.gif", Description = "Step forward with one leg, lower your hips until both knees are at 90 degrees.", IsPremium = false },
                new Exercise { ExerciseId = 5, Name = "Deadlift", MuscleGroup = "Back", Difficulty = "Advanced", Equipment = "Dumbbell", GifUrl = "https://media.giphy.com/media/deadlift.gif", Description = "Advanced strength exercise targeting the posterior chain.", IsPremium = true }
            );
        }
    }
}