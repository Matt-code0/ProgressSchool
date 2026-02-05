using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progress.Data;
using Progress.DTO;
using Progress.Models;
using Progress.Services;
using System.Security.Claims;

[Authorize]
public class WorkoutLogController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly DashboardService _service;

    public WorkoutLogController(ApplicationDbContext context, DashboardService service)
    {
        _context = context;
        _service = service;
    }

    // ============================
    // EDIT / ACTIVE WORKOUT
    // ============================
    public async Task<IActionResult> Edit(int id)
    {
        var workout = await _context.WorkoutLogs
            .Include(w => w.WorkoutTemplate)
            .Include(w => w.Exercises)
                .ThenInclude(e => e.ExerciseTemplate)
            .Include(w => w.Exercises)
                .ThenInclude(e => e.Series)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workout == null) return NotFound();

        return View(ToDto(workout));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(WorkoutLogDTO dto)
    {
        var workout = await _context.WorkoutLogs
            .Include(w => w.Exercises)
                .ThenInclude(e => e.Series)
            .FirstOrDefaultAsync(w => w.Id == dto.Id);

        if (workout == null) return NotFound();

        foreach (var ex in dto.Exercises)
        {
            var logEx = workout.Exercises.First(e => e.Id == ex.Id);
            foreach (var s in ex.Series)
            {
                var logS = logEx.Series.First(x => x.Id == s.Id);
                logS.Reps = s.Reps ?? 0;
                logS.Weight = s.Weight ?? 0;
                logS.Note = s.Note ?? "";
            }
        }

        workout.Completed = true;
        workout.Date = DateTime.Now;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Dashboard");
    }

    // ============================
    // FINISHED WORKOUT DETAIL
    // ============================
    public async Task<IActionResult> FinishedWorkoutDetail(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var workout = await _service.GetFinishedWorkoutAsync(id, userId);

        if (workout == null) return NotFound();

        return View(ToDto(workout));
    }

    // ============================
    // START FROM FINISHED
    // ============================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartFromFinished(int finishedWorkoutId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var dto = await _service.StartWorkoutFromFinishedAsync(finishedWorkoutId, userId);

        return RedirectToAction("Edit", new { id = dto.Id });
    }

    private static WorkoutLogDTO ToDto(WorkoutLog w)
    {
        return new WorkoutLogDTO
        {
            Id = w.Id,
            TemplateName = w.WorkoutTemplate?.Name,
            StartedAt = w.Date,
            Exercises = w.Exercises.Select(e => new ExerciseLogDTO
            {
                Id = e.Id,
                ExerciseName = e.ExerciseTemplate?.Name ?? e.Name,
                Series = e.Series.Select(s => new ExerciseSeriesLogDTO
                {
                    Id = s.Id,
                    Reps = s.Reps,
                    Weight = s.Weight,
                    Note = s.Note
                }).ToList()
            }).ToList()
        };
    }
}
