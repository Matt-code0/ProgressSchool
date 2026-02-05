namespace Progress.Models
{
    public class ExerciseTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WorkoutTemplateId { get; set; }
        public WorkoutTemplate WorkoutTemplate { get; set; }
        public List<ExerciseSeriesTemplate> Series { get; set; } = new();
    }
}
