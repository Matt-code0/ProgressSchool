namespace Progress.Models
{
    public class ExerciseSeriesTemplate
    {
        public int Id { get; set; }
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
        public int ExerciseTemplateId { get; set; }
        public ExerciseTemplate ExerciseTemplate { get; set; }
    }
}
