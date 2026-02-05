namespace Progress.DTO
{
    public class WorkoutTemplateDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Split { get; set; }
        public List<ExerciseTemplateDTO> Exercises { get; set; } = new();
    }
}
