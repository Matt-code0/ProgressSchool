namespace Progress.Models
{
    public class ExerciseLog
    {
        public int Id { get; set; }
        public int ExerciseTemplateId { get; set; }
        public ExerciseTemplate ExerciseTemplate { get; set; }

        public int WorkoutLogId { get; set; }
        public WorkoutLog WorkoutLog { get; set; }

        public string Name { get; set; }
        public string Notes { get; set; }
        public ICollection<ExerciseSeriesLog> Series { get; set; }
    }
}
