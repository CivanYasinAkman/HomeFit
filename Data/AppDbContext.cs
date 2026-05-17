using System; // DateTime kullanımı için gereken kütüphane eklendi
using Microsoft.EntityFrameworkCore;
using HomeFit.Models;

namespace HomeFit.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutProgram> WorkoutPrograms { get; set; }
        public DbSet<UserProgram> UserPrograms { get; set; }
        public DbSet<ProgramExercise> ProgramExercises { get; set; }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
        public DbSet<SessionExercise> SessionExercises { get; set; }
        public DbSet<Progress> ProgressLogs { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User - UserProgram (1-1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProgram)
                .WithOne(up => up.User)
                .HasForeignKey<UserProgram>(up => up.UserId);

            // User - Subscription (1-1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Subscription)
                .WithOne(s => s.User)
                .HasForeignKey<Subscription>(s => s.UserId);

            // User - WorkoutSessions (1-many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.WorkoutSessions)
                .WithOne(ws => ws.User)
                .HasForeignKey(ws => ws.UserId);

            // User - ProgressLogs (1-many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.ProgressLogs)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            // WorkoutSession - SessionExercises (1-many)
            modelBuilder.Entity<WorkoutSession>()
                .HasMany(ws => ws.SessionExercises)
                .WithOne(se => se.Session)
                .HasForeignKey(se => se.SessionId);

            // WorkoutProgram - ProgramExercises (1-many)
            modelBuilder.Entity<WorkoutProgram>()
                .HasMany(wp => wp.ProgramExercises)
                .WithOne(pe => pe.Program)
                .HasForeignKey(pe => pe.ProgramId);

            // Seed: Exercises
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise { ExerciseId = 1, Name = "Squat", MuscleGroup = "Legs", Difficulty = "Beginner", Equipment = "None", GifUrl = "https://media.giphy.com/media/squat.gif", Description = "Stand with feet shoulder-width apart, lower your body as if sitting back into a chair.", IsPremium = false },
                new Exercise { ExerciseId = 2, Name = "Push-up", MuscleGroup = "Chest", Difficulty = "Beginner", Equipment = "None", GifUrl = "https://media.giphy.com/media/pushup.gif", Description = "Keep your body straight, lower your chest to the floor and push back up.", IsPremium = false },
                new Exercise { ExerciseId = 3, Name = "Plank", MuscleGroup = "Core", Difficulty = "Beginner", Equipment = "None", GifUrl = "https://media.giphy.com/media/plank.gif", Description = "Hold a push-up position with your body in a straight line.", IsPremium = false },
                new Exercise { ExerciseId = 4, Name = "Lunges", MuscleGroup = "Legs", Difficulty = "Beginner", Equipment = "None", GifUrl = "https://media.giphy.com/media/lunges.gif", Description = "Step forward with one leg, lower your hips until both knees are at 90 degrees.", IsPremium = false },
                new Exercise { ExerciseId = 5, Name = "Burpee", MuscleGroup = "Full Body", Difficulty = "Intermediate", Equipment = "None", GifUrl = "https://media.giphy.com/media/burpee.gif", Description = "Drop into a squat, kick feet back, do a push-up, return and jump.", IsPremium = false },
                new Exercise { ExerciseId = 6, Name = "Mountain Climber", MuscleGroup = "Core", Difficulty = "Intermediate", Equipment = "None", GifUrl = "https://media.giphy.com/media/mountainclimber.gif", Description = "In plank position, alternate driving knees toward your chest rapidly.", IsPremium = false },
                new Exercise { ExerciseId = 7, Name = "Deadlift", MuscleGroup = "Back", Difficulty = "Advanced", Equipment = "Dumbbell", GifUrl = "https://media.giphy.com/media/deadlift.gif", Description = "Advanced strength exercise targeting the posterior chain.", IsPremium = true },
                new Exercise { ExerciseId = 8, Name = "Pull-up", MuscleGroup = "Back", Difficulty = "Advanced", Equipment = "Pull-up Bar", GifUrl = "https://media.giphy.com/media/pullup.gif", Description = "Hang from a bar and pull your chin above it.", IsPremium = true }
            );

            // Seed: WorkoutPrograms
            modelBuilder.Entity<WorkoutProgram>().HasData(
                new WorkoutProgram { ProgramId = 1, Name = "Beginner Fat Loss", Description = "3 days/week bodyweight cardio", DifficultyLevel = "beginner", Goal = "fat_loss", DurationWeeks = 4, IsPremium = false },
                new WorkoutProgram { ProgramId = 2, Name = "Beginner Muscle Gain", Description = "3 days/week basic strength", DifficultyLevel = "beginner", Goal = "muscle_gain", DurationWeeks = 4, IsPremium = false },
                new WorkoutProgram { ProgramId = 3, Name = "Intermediate Fat Loss", Description = "4 days/week HIIT + strength", DifficultyLevel = "intermediate", Goal = "fat_loss", DurationWeeks = 6, IsPremium = false },
                new WorkoutProgram { ProgramId = 4, Name = "Intermediate Muscle Gain", Description = "4 days/week progressive overload", DifficultyLevel = "intermediate", Goal = "muscle_gain", DurationWeeks = 6, IsPremium = true },
                new WorkoutProgram { ProgramId = 5, Name = "Advanced Strength", Description = "5 days/week heavy compound lifts", DifficultyLevel = "advanced", Goal = "muscle_gain", DurationWeeks = 8, IsPremium = true },
                new WorkoutProgram { ProgramId = 6, Name = "General Fitness", Description = "3 days/week full body", DifficultyLevel = "beginner", Goal = "maintain", DurationWeeks = 4, IsPremium = false }
            );

            // Seed: Admin User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Name = "Admin",
                    Email = "admin@homefit.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!", 10),
                    SecurityQuestion = "Doğduğunuz hastane nedir?",
                    SecurityAnswer = "admin",
                    Role = "Admin",
                    MembershipTier = "Premium",
                    JoinDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    OnboardingCompleted = true
                }
            );

          // Seed: ProgramExercises — Beginner Fat Loss (ProgramId=1)
            modelBuilder.Entity<ProgramExercise>().HasData(
                // Pazartesi
                new ProgramExercise { ProgramExerciseId = 1, ProgramId = 1, ExerciseId = 5, DayOfWeek = "Pazartesi", OrderInDay = 1, Sets = 3, Reps = 15 },
                new ProgramExercise { ProgramExerciseId = 2, ProgramId = 1, ExerciseId = 1, DayOfWeek = "Pazartesi", OrderInDay = 2, Sets = 3, Reps = 15 },
                new ProgramExercise { ProgramExerciseId = 3, ProgramId = 1, ExerciseId = 3, DayOfWeek = "Pazartesi", OrderInDay = 3, Sets = 3, Reps = 30 },
                // Çarşamba
                new ProgramExercise { ProgramExerciseId = 4, ProgramId = 1, ExerciseId = 6, DayOfWeek = "Çarşamba", OrderInDay = 1, Sets = 3, Reps = 20 },
                new ProgramExercise { ProgramExerciseId = 5, ProgramId = 1, ExerciseId = 2, DayOfWeek = "Çarşamba", OrderInDay = 2, Sets = 3, Reps = 12 },
                new ProgramExercise { ProgramExerciseId = 6, ProgramId = 1, ExerciseId = 4, DayOfWeek = "Çarşamba", OrderInDay = 3, Sets = 3, Reps = 12 },
                // Cuma
                new ProgramExercise { ProgramExerciseId = 7, ProgramId = 1, ExerciseId = 5, DayOfWeek = "Cuma", OrderInDay = 1, Sets = 4, Reps = 15 },
                new ProgramExercise { ProgramExerciseId = 8, ProgramId = 1, ExerciseId = 1, DayOfWeek = "Cuma", OrderInDay = 2, Sets = 4, Reps = 15 },
                new ProgramExercise { ProgramExerciseId = 9, ProgramId = 1, ExerciseId = 3, DayOfWeek = "Cuma", OrderInDay = 3, Sets = 3, Reps = 40 },

                // Seed: Beginner Muscle Gain (ProgramId=2)
                // Pazartesi
                new ProgramExercise { ProgramExerciseId = 10, ProgramId = 2, ExerciseId = 2, DayOfWeek = "Pazartesi", OrderInDay = 1, Sets = 4, Reps = 10 },
                new ProgramExercise { ProgramExerciseId = 11, ProgramId = 2, ExerciseId = 1, DayOfWeek = "Pazartesi", OrderInDay = 2, Sets = 4, Reps = 12 },
                new ProgramExercise { ProgramExerciseId = 12, ProgramId = 2, ExerciseId = 3, DayOfWeek = "Pazartesi", OrderInDay = 3, Sets = 3, Reps = 30 },
                // Çarşamba
                new ProgramExercise { ProgramExerciseId = 13, ProgramId = 2, ExerciseId = 4, DayOfWeek = "Çarşamba", OrderInDay = 1, Sets = 4, Reps = 10 },
                new ProgramExercise { ProgramExerciseId = 14, ProgramId = 2, ExerciseId = 2, DayOfWeek = "Çarşamba", OrderInDay = 2, Sets = 3, Reps = 12 },
                // Cuma
                new ProgramExercise { ProgramExerciseId = 15, ProgramId = 2, ExerciseId = 1, DayOfWeek = "Cuma", OrderInDay = 1, Sets = 4, Reps = 15 },
                new ProgramExercise { ProgramExerciseId = 16, ProgramId = 2, ExerciseId = 3, DayOfWeek = "Cuma", OrderInDay = 2, Sets = 4, Reps = 30 },
                new ProgramExercise { ProgramExerciseId = 17, ProgramId = 2, ExerciseId = 2, DayOfWeek = "Cuma", OrderInDay = 3, Sets = 4, Reps = 10 },

                // Seed: Intermediate Fat Loss (ProgramId=3)
                new ProgramExercise { ProgramExerciseId = 18, ProgramId = 3, ExerciseId = 5, DayOfWeek = "Pazartesi", OrderInDay = 1, Sets = 4, Reps = 15 },
                new ProgramExercise { ProgramExerciseId = 19, ProgramId = 3, ExerciseId = 6, DayOfWeek = "Pazartesi", OrderInDay = 2, Sets = 4, Reps = 20 },
                new ProgramExercise { ProgramExerciseId = 20, ProgramId = 3, ExerciseId = 2, DayOfWeek = "Salı", OrderInDay = 1, Sets = 4, Reps = 15 },
                new ProgramExercise { ProgramExerciseId = 21, ProgramId = 3, ExerciseId = 1, DayOfWeek = "Salı", OrderInDay = 2, Sets = 4, Reps = 20 },
                new ProgramExercise { ProgramExerciseId = 22, ProgramId = 3, ExerciseId = 5, DayOfWeek = "Perşembe", OrderInDay = 1, Sets = 5, Reps = 15 },
                new ProgramExercise { ProgramExerciseId = 23, ProgramId = 3, ExerciseId = 3, DayOfWeek = "Perşembe", OrderInDay = 2, Sets = 4, Reps = 40 },
                new ProgramExercise { ProgramExerciseId = 24, ProgramId = 3, ExerciseId = 6, DayOfWeek = "Cumartesi", OrderInDay = 1, Sets = 5, Reps = 20 },
                new ProgramExercise { ProgramExerciseId = 25, ProgramId = 3, ExerciseId = 4, DayOfWeek = "Cumartesi", OrderInDay = 2, Sets = 4, Reps = 15 },

                // Seed: General Fitness (ProgramId=6)
                new ProgramExercise { ProgramExerciseId = 26, ProgramId = 6, ExerciseId = 1, DayOfWeek = "Pazartesi", OrderInDay = 1, Sets = 3, Reps = 12 },
                new ProgramExercise { ProgramExerciseId = 27, ProgramId = 6, ExerciseId = 2, DayOfWeek = "Pazartesi", OrderInDay = 2, Sets = 3, Reps = 10 },
                new ProgramExercise { ProgramExerciseId = 28, ProgramId = 6, ExerciseId = 3, DayOfWeek = "Pazartesi", OrderInDay = 3, Sets = 3, Reps = 30 },
                new ProgramExercise { ProgramExerciseId = 29, ProgramId = 6, ExerciseId = 4, DayOfWeek = "Çarşamba", OrderInDay = 1, Sets = 3, Reps = 12 },
                new ProgramExercise { ProgramExerciseId = 30, ProgramId = 6, ExerciseId = 5, DayOfWeek = "Çarşamba", OrderInDay = 2, Sets = 3, Reps = 10 },
                new ProgramExercise { ProgramExerciseId = 31, ProgramId = 6, ExerciseId = 1, DayOfWeek = "Cuma", OrderInDay = 1, Sets = 3, Reps = 15 },
                new ProgramExercise { ProgramExerciseId = 32, ProgramId = 6, ExerciseId = 2, DayOfWeek = "Cuma", OrderInDay = 2, Sets = 3, Reps = 12 }
            );  
        }
    }
}