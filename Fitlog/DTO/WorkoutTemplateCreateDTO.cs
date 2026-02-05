namespace Progress.DTO
{
    public class WorkoutTemplateCreateDTO
    {
        public string Name { get; set; }
        public string Split { get; set; }

        // Uvnitř bude seznam cvičení
        public List<ExerciseTemplateDTO> Exercises { get; set; } = new();
    }
}
