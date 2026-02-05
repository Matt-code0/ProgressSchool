using System.Collections.Generic;

namespace Progress.Models
{
    public class WorkoutTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Split { get; set; }
        public List<ExerciseTemplate> Exercises { get; set; } = new();
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public List<WorkoutLog> WorkoutLogs { get; set; } = new();

        // NOVÉ: soft delete
        public bool IsDeleted { get; set; } = false;

        // Pomocné: kontrola, jestli template byla použita
        public bool HasFinishedWorkout => WorkoutLogs != null && WorkoutLogs.Count > 0;
    }
}
