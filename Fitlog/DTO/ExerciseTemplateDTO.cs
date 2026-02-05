using System.ComponentModel.DataAnnotations;

namespace Progress.DTO
{
    public class ExerciseTemplateDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Exercise name is required.")]
        public string Name { get; set; }

        public List<ExerciseSeriesTemplateDTO> Series { get; set; } = new();
    }
}
