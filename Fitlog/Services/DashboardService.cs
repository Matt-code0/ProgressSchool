using Microsoft.EntityFrameworkCore;
using Progress.DTO;
using Progress.Models;
using Progress.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Progress.Services
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================
        // Načte všechny Workout Templates pro konkrétního uživatele
        // ============================
        public async Task<List<WorkoutTemplate>> GetUserTemplatesAsync(string userId)
        {
            return await _context.WorkoutTemplates
                .Where(t => !t.IsDeleted && t.UserId == userId) // pouze aktivní a uživateli patřící šablony
                .Include(t => t.Exercises)
                    .ThenInclude(e => e.Series)
                .ToListAsync();
        }

        // ============================
        // Aktivní/neukončený workout
        // ============================
        public async Task<WorkoutLog?> GetActiveWorkoutAsync(string userId)
        {
            return await _context.WorkoutLogs
                .Include(w => w.Exercises)
                    .ThenInclude(e => e.Series)
                .Include(w => w.WorkoutTemplate)
                .Where(w => w.UserId == userId && !w.Completed)
                .OrderByDescending(w => w.Date)
                .FirstOrDefaultAsync();
        }

        // ============================
        // Poslední relevantní workout (aktivní nebo poslední dokončený)
        // ============================
        public async Task<WorkoutLog?> GetLastRelevantWorkoutAsync(string userId)
        {
            var active = await GetActiveWorkoutAsync(userId);
            if (active != null)
                return active;

            return await _context.WorkoutLogs
                .Include(w => w.Exercises)
                    .ThenInclude(e => e.Series)
                .Include(w => w.WorkoutTemplate)
                .Where(w => w.UserId == userId && w.Completed)
                .OrderByDescending(w => w.Date)
                .FirstOrDefaultAsync();
        }

        // ============================
        // Start Workout – zkopíruje šablonu do nového WorkoutLogu
        // ============================
        public async Task<WorkoutLogDTO> StartWorkoutAsync(int templateId, string userId)
        {
            var template = await _context.WorkoutTemplates
                .Include(t => t.Exercises)
                    .ThenInclude(e => e.Series)
                .FirstOrDefaultAsync(t => t.Id == templateId && t.UserId == userId);

            if (template == null)
                throw new Exception("Template not found.");

            var workoutLog = new WorkoutLog
            {
                UserId = userId,
                WorkoutTemplateId = template.Id,
                Date = DateTime.Now,
                Split = template.Split,
                Completed = false,
                Exercises = template.Exercises.Select(ex => new ExerciseLog
                {
                    ExerciseTemplateId = ex.Id,
                    Name = ex.Name,
                    Notes = string.Empty,
                    Series = ex.Series.Select(s => new ExerciseSeriesLog
                    {
                        Reps = s.Reps,
                        Weight = s.Weight,
                        Note = string.Empty
                    }).ToList()
                }).ToList()
            };

            _context.WorkoutLogs.Add(workoutLog);
            await _context.SaveChangesAsync();

            return MapToDTO(workoutLog, template.Name);
        }

        // ============================
        // Start Workout z Finished Workout – kopíruje do nového workoutu
        // ============================
        public async Task<WorkoutLogDTO> StartWorkoutFromFinishedAsync(int finishedWorkoutId, string userId)
        {
            var finishedWorkout = await GetFinishedWorkoutAsync(finishedWorkoutId, userId);

            if (finishedWorkout == null)
                throw new Exception("Finished workout not found.");

            var workoutLog = new WorkoutLog
            {
                UserId = userId,
                WorkoutTemplateId = finishedWorkout.WorkoutTemplateId,
                Date = DateTime.Now,
                Split = finishedWorkout.Split,
                Completed = false,
                Exercises = finishedWorkout.Exercises.Select(ex => new ExerciseLog
                {
                    ExerciseTemplateId = ex.ExerciseTemplateId,
                    Name = ex.Name,
                    Notes = string.Empty,
                    Series = ex.Series.Select(s => new ExerciseSeriesLog
                    {
                        Reps = s.Reps,
                        Weight = s.Weight,
                        Note = s.Note
                    }).ToList()
                }).ToList()
            };

            _context.WorkoutLogs.Add(workoutLog);
            await _context.SaveChangesAsync();

            return MapToDTO(workoutLog, finishedWorkout.WorkoutTemplate?.Name ?? "Workout");
        }

        // ============================
        // Posledních 7 finished workoutů
        // ============================
        public async Task<List<WorkoutLog>> GetFinishedWorkoutsAsync(string userId, int count = 7)
        {
            return await _context.WorkoutLogs
                .Include(w => w.Exercises)
                    .ThenInclude(e => e.Series)
                .Include(w => w.WorkoutTemplate)
                .Where(w => w.UserId == userId && w.Completed)
                .OrderByDescending(w => w.Date)
                .Take(count)
                .ToListAsync();
        }

        // ============================
        // Načte jeden Finished Workout podle ID
        // ============================
        public async Task<WorkoutLog?> GetFinishedWorkoutAsync(int finishedWorkoutId, string userId)
        {
            return await _context.WorkoutLogs
                .Include(w => w.Exercises)
                    .ThenInclude(e => e.Series)
                .Include(w => w.WorkoutTemplate)
                .FirstOrDefaultAsync(w => w.Id == finishedWorkoutId
                    && w.UserId == userId
                    && w.Completed);
        }

        // ============================
        // Pomocná metoda pro mapování na DTO
        // ============================
        public WorkoutLogDTO MapToDTO(WorkoutLog workoutLog, string templateName)
        {
            return new WorkoutLogDTO
            {
                Id = workoutLog.Id,
                TemplateName = templateName,
                StartedAt = workoutLog.Date,
                Exercises = workoutLog.Exercises.Select(ex => new ExerciseLogDTO
                {
                    Id = ex.Id,
                    ExerciseName = ex.Name,
                    Series = ex.Series.Select(s => new ExerciseSeriesLogDTO
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
}
