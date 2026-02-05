using Progress.DTO;
using Progress.Models;
using System.Collections.Generic;

namespace Progress.ViewModels
{
    public class DashboardViewModel
    {
        public List<WorkoutTemplate>? Templates { get; set; }
        public WorkoutLogDTO? LastWorkout { get; set; }

        // Nové: Finished Workouts
        public List<WorkoutLogDTO>? FinishedWorkouts { get; set; }
    }
}
