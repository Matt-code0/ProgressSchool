using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.DTO;
using Progress.Services;
using System.Security.Claims;

[Authorize]
public class WorkoutTemplateController : Controller
{
    private readonly IWorkoutTemplateService _service;

    public WorkoutTemplateController(IWorkoutTemplateService service)
    {
        _service = service;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    public async Task<IActionResult> Index()
    {
        var templates = await _service.GetAllAsync(GetUserId());
        return View(templates);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.WarningMessage =
            "Warning! You can edit template before clicking START in dashboard. Once you start a workout, you won't be able to edit it anymore!";
        return View(new WorkoutTemplateDTO { Exercises = new List<ExerciseTemplateDTO>() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutTemplateDTO dto)
    {
        if (!ModelState.IsValid) return View(dto);

        dto.Exercises = dto.Exercises.Where(e => !string.IsNullOrWhiteSpace(e.Name)).ToList();
        if (!dto.Exercises.Any())
        {
            ModelState.AddModelError("Exercises", "Workout template must contain at least one exercise.");
            return View(dto);
        }

        foreach (var ex in dto.Exercises)
        {
            ex.Series = ex.Series?.Where(s => s.Weight.HasValue && s.Reps.HasValue).ToList() ?? new List<ExerciseSeriesTemplateDTO>();
            if (!ex.Series.Any())
            {
                ModelState.AddModelError("Exercises", "Each exercise must contain at least one set with weight and reps.");
                return View(dto);
            }
        }

        var createdId = await _service.CreateAsync(dto, GetUserId());
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var template = await _service.GetByIdAsync(id, GetUserId());
        if (template == null) return NotFound();

        template.Exercises ??= new List<ExerciseTemplateDTO>();

        // Soft delete aktivuje se až po START
        var hasStartedWorkout = await _service.TemplateHasFinishedWorkoutAsync(id, GetUserId());
        ViewBag.IsSoftDeleted = hasStartedWorkout;

        // Varování
        if (!hasStartedWorkout)
        {
            ViewBag.WarningMessage = "Warning! You can edit before clicking START in dashboard. Once you start a workout, you won't be able to edit it anymore!";
        }
        else
        {
            ViewBag.WarningMessage = "Warning: This template has already been used. Once you start a workout from it, you won't be able to edit it anymore.";
        }

        return View(template);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WorkoutTemplateDTO dto)
    {
        var template = await _service.GetByIdAsync(id, GetUserId());
        if (template == null) return NotFound();

        var hasStartedWorkout = await _service.TemplateHasFinishedWorkoutAsync(id, GetUserId());
        ViewBag.IsSoftDeleted = hasStartedWorkout;

        // Pokud soft delete (po START), edit není povolen
        if (hasStartedWorkout)
        {
            ModelState.AddModelError("", "You cannot edit this template because it has already been used.");
            ViewBag.WarningMessage = "Warning: This template has already been used. Once you start a workout from it, you won't be able to edit it anymore.";
            return View(dto);
        }

        // HARD edit validace
        if (!ModelState.IsValid)
        {
            dto.Exercises ??= new List<ExerciseTemplateDTO>();
            foreach (var ex in dto.Exercises)
                ex.Series ??= new List<ExerciseSeriesTemplateDTO>();
            return View(dto);
        }

        var success = await _service.UpdateAsync(id, dto, GetUserId());
        if (!success)
        {
            ModelState.AddModelError("", "You cannot edit this template because it has already been used.");
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, GetUserId());
        return RedirectToAction(nameof(Index));
    }
}
