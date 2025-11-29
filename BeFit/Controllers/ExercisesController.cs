using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BeFit.Controllers
{
    [Authorize] // [Wymagane] Tylko zalogowani
    public class ExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExercisesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Exercises
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            // [Wymagane] Filtrowanie po sesjach użytkownika
            var exercises = _context.TrainingExercises
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .Where(e => e.TrainingSession.UserId == user.Id);

            return View(await exercises.ToListAsync());
        }

        // GET: Exercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.GetUserAsync(User);

            var exercise = await _context.TrainingExercises
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .FirstOrDefaultAsync(m => m.Id == id && m.TrainingSession.UserId == user.Id);

            if (exercise == null) return NotFound();

            return View(exercise);
        }

        // GET: Exercises/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name"); // [Wymagane] Nazwa zamiast ID

            // [Wymagane] Tylko własne sesje i data zamiast ID
            ViewData["TrainingSessionId"] = new SelectList(
                _context.TrainingSessions.Where(s => s.UserId == user.Id),
                "Id",
                "StartTime");

            return View();
        }

        // POST: Exercises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Weight,Sets,Reps,ExerciseTypeId,TrainingSessionId")] Exercise exercise)
        {
            var user = await _userManager.GetUserAsync(User);

            // Weryfikacja czy sesja należy do usera
            var session = await _context.TrainingSessions.FindAsync(exercise.TrainingSessionId);
            if (session == null || session.UserId != user.Id)
            {
                ModelState.AddModelError("", "Nieprawidłowa sesja treningowa.");
            }

            // Ignorowanie pól nawigacyjnych
            ModelState.Remove("ExerciseType");
            ModelState.Remove("TrainingSession");

            if (ModelState.IsValid)
            {
                _context.Add(exercise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", exercise.ExerciseTypeId);
            ViewData["TrainingSessionId"] = new SelectList(
                _context.TrainingSessions.Where(s => s.UserId == user.Id),
                "Id",
                "StartTime",
                exercise.TrainingSessionId);
            return View(exercise);
        }

        // GET: Exercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.GetUserAsync(User);

            var exercise = await _context.TrainingExercises
                .Include(e => e.TrainingSession)
                .FirstOrDefaultAsync(e => e.Id == id && e.TrainingSession.UserId == user.Id);

            if (exercise == null) return NotFound();

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", exercise.ExerciseTypeId);
            ViewData["TrainingSessionId"] = new SelectList(
                _context.TrainingSessions.Where(s => s.UserId == user.Id),
                "Id",
                "StartTime",
                exercise.TrainingSessionId);
            return View(exercise);
        }

        // POST: Exercises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Weight,Sets,Reps,ExerciseTypeId,TrainingSessionId")] Exercise exercise)
        {
            if (id != exercise.Id) return NotFound();

            var user = await _userManager.GetUserAsync(User);

            // Weryfikacja uprawnień do starej i nowej sesji
            var session = await _context.TrainingSessions.FindAsync(exercise.TrainingSessionId);
            if (session == null || session.UserId != user.Id)
            {
                return Forbid();
            }

            ModelState.Remove("ExerciseType");
            ModelState.Remove("TrainingSession");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exercise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExerciseExists(exercise.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", exercise.ExerciseTypeId);
            ViewData["TrainingSessionId"] = new SelectList(_context.TrainingSessions.Where(s => s.UserId == user.Id), "Id", "StartTime", exercise.TrainingSessionId);
            return View(exercise);
        }

        // GET: Exercises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.GetUserAsync(User);

            var exercise = await _context.TrainingExercises
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .FirstOrDefaultAsync(m => m.Id == id && m.TrainingSession.UserId == user.Id);

            if (exercise == null) return NotFound();

            return View(exercise);
        }

        // POST: Exercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var exercise = await _context.TrainingExercises
                .Include(e => e.TrainingSession)
                .FirstOrDefaultAsync(e => e.Id == id && e.TrainingSession.UserId == user.Id);

            if (exercise != null)
            {
                _context.TrainingExercises.Remove(exercise);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ExerciseExists(int id)
        {
            return _context.TrainingExercises.Any(e => e.Id == id);
        }
    }
}
