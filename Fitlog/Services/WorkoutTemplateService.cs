using Microsoft.EntityFrameworkCore;
using Progress.Data;
using Progress.DTO;
using Progress.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Progress.Services
{
    public class WorkoutTemplateService : IWorkoutTemplateService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutTemplateService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkoutTemplateDTO>> GetAllAsync(string userId)
        {
            return await _context.WorkoutTemplates
                .Where(t => t.UserId == userId && !t.IsDeleted)
                .Include(t => t.Exercises)
                    .ThenInclude(e => e.Series)
                .Select(t => new WorkoutTemplateDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    Split = t.Split,
                    Exercises = t.Exercises.Select(e => new ExerciseTemplateDTO
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Series = e.Series.Select(s => new ExerciseSeriesTemplateDTO
                        {
                            Id = s.Id,
                            SetNumber = s.SetNumber,
                            Reps = s.Reps,
                            Weight = s.Weight
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<WorkoutTemplateDTO> GetByIdAsync(int id, string userId)
        {
            var template = await _context.WorkoutTemplates
                .Where(t => t.Id == id && t.UserId == userId)
                .Include(t => t.Exercises)
                    .ThenInclude(e => e.Series)
                .FirstOrDefaultAsync();

            if (template == null) return null;

            return new WorkoutTemplateDTO
            {
                Id = template.Id,
                Name = template.Name,
                Split = template.Split,
                Exercises = template.Exercises.Select(e => new ExerciseTemplateDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Series = e.Series.Select(s => new ExerciseSeriesTemplateDTO
                    {
                        Id = s.Id,
                        SetNumber = s.SetNumber,
                        Reps = s.Reps,
                        Weight = s.Weight
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<int> CreateAsync(WorkoutTemplateDTO dto, string userId)
        {
            var validExercises = dto.Exercises
                .Where(e => !string.IsNullOrWhiteSpace(e.Name))
                .Select(e => new ExerciseTemplate
                {
                    Name = e.Name.Trim(),
                    Series = e.Series
                        .Where(s => s.Reps.HasValue && s.Weight.HasValue)
                        .Select((s, index) => new ExerciseSeriesTemplate
                        {
                            SetNumber = index + 1,
                            Reps = s.Reps.GetValueOrDefault(10),
                            Weight = s.Weight.GetValueOrDefault(0)
                        }).ToList()
                })
                .Where(e => e.Series.Any())
                .ToList();

            if (!validExercises.Any())
                return 0;

            var template = new WorkoutTemplate
            {
                Name = dto.Name.Trim(),
                Split = dto.Split.Trim(),
                UserId = userId,
                Exercises = validExercises
            };

            _context.WorkoutTemplates.Add(template);
            await _context.SaveChangesAsync();

            return template.Id;
        }

        public async Task<bool> UpdateAsync(int id, WorkoutTemplateDTO dto, string userId)
        {
            var template = await _context.WorkoutTemplates
                .Include(t => t.Exercises)
                    .ThenInclude(e => e.Series)
                .Include(t => t.WorkoutLogs)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (template == null) return false;

            // Blokovat edit pokud template byla použita
            if (template.HasFinishedWorkout) return false;

            template.Name = dto.Name;
            template.Split = dto.Split;

            // Odstranit staré sety a cvičení
            _context.ExerciseSeriesTemplates.RemoveRange(template.Exercises.SelectMany(e => e.Series));
            _context.ExerciseTemplates.RemoveRange(template.Exercises);

            // Připrav nové cvičení se sety
            var newExercises = dto.Exercises
                .Where(e => !string.IsNullOrWhiteSpace(e.Name))
                .Select(e =>
                {
                    var validSeries = e.Series
                        .Where(s => s.Reps.HasValue && s.Weight.HasValue)
                        .Select((s, idx) => new ExerciseSeriesTemplate
                        {
                            SetNumber = idx + 1,
                            Reps = s.Reps.GetValueOrDefault(10),
                            Weight = s.Weight.GetValueOrDefault(0)
                        }).ToList();

                    return new ExerciseTemplate
                    {
                        Name = e.Name.Trim(),
                        Series = validSeries
                    };
                })
                .Where(e => e.Series.Any())
                .ToList();

            template.Exercises = newExercises;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var template = await _context.WorkoutTemplates
                .Include(t => t.Exercises)
                    .ThenInclude(e => e.Series)
                .Include(t => t.WorkoutLogs)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (template == null) return false;

            if (template.HasFinishedWorkout)
            {
                // soft delete
                template.IsDeleted = true;
            }
            else
            {
                // hard delete
                _context.WorkoutTemplates.Remove(template);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> TemplateHasFinishedWorkoutAsync(int id, string userId)
        {
            var template = await _context.WorkoutTemplates
                .Include(t => t.WorkoutLogs)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (template == null) return false;

            return template.WorkoutLogs != null && template.WorkoutLogs.Count > 0;
        }
    }
}
