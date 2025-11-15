using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;

namespace BeFit.Controllers
{
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var fourWeeksAgo = DateTime.Now.AddDays(-28);

            var stats = await _context.ExerciseTypes
                .Select(type => new StatsViewModel
                {
                    ExerciseName = type.Name,

                    TimesPerformed = _context.TrainingExercises
                        .Where(e => e.ExerciseTypeId == type.Id &&
                                    e.TrainingSession.StartTime >= fourWeeksAgo)
                        .Count(),

                    TotalReps = _context.TrainingExercises
                        .Where(e => e.ExerciseTypeId == type.Id &&
                                    e.TrainingSession.StartTime >= fourWeeksAgo)
                        .Sum(e => e.Sets * e.Reps),

                    AverageWeight = _context.TrainingExercises
                        .Where(e => e.ExerciseTypeId == type.Id &&
                                    e.TrainingSession.StartTime >= fourWeeksAgo)
                        .Average(e => (double?)e.Weight) ?? 0,

                    MaxWeight = _context.TrainingExercises
                        .Where(e => e.ExerciseTypeId == type.Id &&
                                    e.TrainingSession.StartTime >= fourWeeksAgo)
                        .Max(e => (double?)e.Weight) ?? 0
                })
                .ToListAsync();

            return View(stats);
        }
    }
}
