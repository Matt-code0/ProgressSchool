using System.ComponentModel.DataAnnotations;

namespace Progress.DTO
{
    public class WorkoutTemplateDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Workout name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Split is required.")]
        public string Split { get; set; }

        public List<ExerciseTemplateDTO> Exercises { get; set; } = new();
    }
}
