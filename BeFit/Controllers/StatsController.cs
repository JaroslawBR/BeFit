using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BeFit.Controllers
{
    [Authorize]
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StatsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var fourWeeksAgo = DateTime.Now.AddDays(-28);


            var stats = await _context.ExerciseTypes
                .Select(type => new StatsViewModel
                {
                    ExerciseName = type.Name,


                    TimesPerformed = _context.TrainingExercises
                        .Where(e => e.ExerciseTypeId == type.Id &&
                                    e.TrainingSession.UserId == user.Id &&
                                    e.TrainingSession.StartTime >= fourWeeksAgo)
                        .Count(),

                    TotalReps = _context.TrainingExercises
                        .Where(e => e.ExerciseTypeId == type.Id &&
                                    e.TrainingSession.UserId == user.Id &&
                                    e.TrainingSession.StartTime >= fourWeeksAgo)
                        .Sum(e => e.Sets * e.Reps),


                    AverageWeight = _context.TrainingExercises
                        .Where(e => e.ExerciseTypeId == type.Id &&
                                    e.TrainingSession.UserId == user.Id &&
                                    e.TrainingSession.StartTime >= fourWeeksAgo)
                        .Average(e => (double?)e.Weight) ?? 0,

                    MaxWeight = _context.TrainingExercises
                        .Where(e => e.ExerciseTypeId == type.Id &&
                                    e.TrainingSession.UserId == user.Id &&
                                    e.TrainingSession.StartTime >= fourWeeksAgo)
                        .Max(e => (double?)e.Weight) ?? 0
                })
                .Where(s => s.TimesPerformed > 0) 
                .ToListAsync();

            return View(stats);
        }
    }
}
