using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Progress.Models;

namespace Progress.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ExerciseTemplate> ExerciseTemplates { get; set; }
        public DbSet<ExerciseSeriesTemplate> ExerciseSeriesTemplates { get; set; }
        public DbSet<ExerciseLog> ExerciseLogs { get; set; }
        public DbSet<ExerciseSeriesLog> ExerciseSeriesLogs { get; set; }
        public DbSet<WorkoutTemplate> WorkoutTemplates { get; set; }
        public DbSet<WorkoutLog> WorkoutLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<WorkoutTemplate>()
               .HasMany(t => t.Exercises)
               .WithOne(e => e.WorkoutTemplate)
               .HasForeignKey(e => e.WorkoutTemplateId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkoutTemplate>()
                .HasMany(t => t.WorkoutLogs)
                .WithOne(w => w.WorkoutTemplate)
                .HasForeignKey(w => w.WorkoutTemplateId)
                .OnDelete(DeleteBehavior.Restrict); // <<<<<<<<<< FINÁLNÍ FIX

            // LOGS → CHILDREN CAN CASCADE DELETE
            builder.Entity<WorkoutLog>()
                .HasMany(w => w.Exercises)
                .WithOne(e => e.WorkoutLog)
                .HasForeignKey(e => e.WorkoutLogId)
                .OnDelete(DeleteBehavior.Restrict);  // ← ZMĚNA !

            builder.Entity<ExerciseLog>()
                .HasMany(e => e.Series)
                .WithOne(s => s.ExerciseLog)
                .HasForeignKey(s => s.ExerciseLogId)
                .OnDelete(DeleteBehavior.Cascade);

            // EXERCISELOG → EXERCISETEMPLATE (MUSÍ BÝT RESTRICT!)
            builder.Entity<ExerciseLog>()
                .HasOne(e => e.ExerciseTemplate)
                .WithMany()
                .HasForeignKey(e => e.ExerciseTemplateId)
                .OnDelete(DeleteBehavior.Restrict); // ← ZMĚNA !
        }
    }
}
