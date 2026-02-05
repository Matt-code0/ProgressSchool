namespace Progress.Models
{
    public class WorkoutLog
    {
        public int Id { get; set; }
        public int WorkoutTemplateId { get; set; }
        public WorkoutTemplate WorkoutTemplate { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime Date { get; set; }
        public string Split { get; set; }
        public bool Completed { get; set; }

        public ICollection<ExerciseLog> Exercises { get; set; }
    }
}
