using Microsoft.AspNetCore.Mvc;
using Progress.Services;
using Progress.DTO;
using Progress.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace Progress.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // ⚡ Změna: načítáme jen šablony aktuálního uživatele
            var templates = await _dashboardService.GetUserTemplatesAsync(userId);

            var lastWorkout = await _dashboardService.GetLastRelevantWorkoutAsync(userId);
            var finishedWorkouts = await _dashboardService.GetFinishedWorkoutsAsync(userId);

            WorkoutLogDTO? lastWorkoutDTO = null;
            if (lastWorkout != null)
            {
                lastWorkoutDTO = _dashboardService.MapToDTO(lastWorkout, lastWorkout.WorkoutTemplate?.Name ?? "Workout");

                // Doplnění Split pro LastWorkout
                lastWorkoutDTO.Split = lastWorkout.WorkoutTemplate?.Split ?? "Unknown";
            }

            // Doplnění Split pro FinishedWorkouts
            var finishedWorkoutsDTO = finishedWorkouts?
                .Select(w =>
                {
                    var dto = _dashboardService.MapToDTO(w, w.WorkoutTemplate?.Name ?? "Workout");
                    dto.Split = w.WorkoutTemplate?.Split ?? "N/A";
                    return dto;
                })
                .ToList();

            var model = new DashboardViewModel
            {
                Templates = templates,
                LastWorkout = lastWorkoutDTO,
                FinishedWorkouts = finishedWorkoutsDTO
            };

            return View(model);
        }

        // POST: /Dashboard/StartWorkout/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartWorkout(int templateId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var workoutDTO = await _dashboardService.StartWorkoutAsync(templateId, userId);

            return RedirectToAction("Edit", "WorkoutLog", new { id = workoutDTO.Id });
        }

        // POST: /Dashboard/StartFromFinished/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartFromFinished(int finishedWorkoutId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var workoutDTO = await _dashboardService.StartWorkoutFromFinishedAsync(finishedWorkoutId, userId);

            return RedirectToAction("Edit", "WorkoutLog", new { id = workoutDTO.Id });
        }
    }
}
