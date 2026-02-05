using System.ComponentModel.DataAnnotations;

namespace Progress.DTO
{
    public class ExerciseLogDTO
    {
        public int? Id { get; set; }
        public string? ExerciseName { get; set; }
        public List<ExerciseSeriesLogDTO>? Series { get; set; } = new();
    }

}
