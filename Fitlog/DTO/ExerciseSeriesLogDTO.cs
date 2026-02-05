using System.ComponentModel.DataAnnotations;

namespace Progress.DTO
{

    public class ExerciseSeriesLogDTO
    {
        public int? Id { get; set; }
        [Range(1, 500, ErrorMessage = "Reps must be at least 1.")]
        public int? Reps { get; set; }
        [Range(0, 500, ErrorMessage = "Weight must be a valid number.")]
        public double? Weight { get; set; }
        public string? Note { get; set; } // <- přidat tady
    }
}
