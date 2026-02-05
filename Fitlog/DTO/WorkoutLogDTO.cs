using System;
using System.Collections.Generic;

namespace Progress.DTO
{
    public class WorkoutLogDTO
    {
        public int? Id { get; set; }
        public string? TemplateName { get; set; }

        // Nové: Split
        public string Split { get; set; }

        public DateTime? StartedAt { get; set; }
        public List<ExerciseLogDTO>? Exercises { get; set; } = new();
    }
}
