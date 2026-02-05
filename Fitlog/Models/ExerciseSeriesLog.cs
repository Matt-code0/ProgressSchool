namespace Progress.Models
{
    public class ExerciseSeriesLog
    {
        public int Id { get; set; }
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
        public bool Completed { get; set; }
        public string Note { get; set; } = string.Empty;
        public int ExerciseLogId { get; set; }
        public ExerciseLog ExerciseLog { get; set; }
    }
}
