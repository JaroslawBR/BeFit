using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BeFit.Controllers
{
    [Authorize] // [Wymagane] Tylko dla zalogowanych
    public class TrainingSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // Menadżer użytkowników

        public TrainingSessionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TrainingSessions
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            // [Wymagane] Filtrowanie po użytkowniku
            return View(await _context.TrainingSessions
                .Where(s => s.UserId == user.Id)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync());
        }

        // GET: TrainingSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.GetUserAsync(User);

            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == user.Id); // Zabezpieczenie dostępu

            if (trainingSession == null) return NotFound();

            return View(trainingSession);
        }

        // GET: TrainingSessions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TrainingSessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartTime,EndTime")] TrainingSession trainingSession)
        {
            // Usuwamy UserId z walidacji, bo ustawiamy go ręcznie
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                trainingSession.UserId = user.Id; // [Wymagane] Wiązanie użytkownika

                _context.Add(trainingSession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainingSession);
        }

        // GET: TrainingSessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.GetUserAsync(User);

            // Zabezpieczenie dostępu
            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == user.Id);

            if (trainingSession == null) return NotFound();
            return View(trainingSession);
        }

        // POST: TrainingSessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartTime,EndTime")] TrainingSession trainingSession)
        {
            if (id != trainingSession.Id) return NotFound();

            // Pobieramy użytkownika, by upewnić się, że edytuje swoje
            var user = await _userManager.GetUserAsync(User);
            var existingSession = await _context.TrainingSessions.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == user.Id);

            if (existingSession == null) return NotFound(); // Próba edycji cudzej sesji

            // Przywracamy UserId, bo formularz go nie przesyła
            trainingSession.UserId = user.Id;
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainingSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingSessionExists(trainingSession.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trainingSession);
        }

        // GET: TrainingSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.GetUserAsync(User);

            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == user.Id); // Zabezpieczenie

            if (trainingSession == null) return NotFound();

            return View(trainingSession);
        }

        // POST: TrainingSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == user.Id);

            if (trainingSession != null)
            {
                _context.TrainingSessions.Remove(trainingSession);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingSessionExists(int id)
        {
            return _context.TrainingSessions.Any(e => e.Id == id);
        }
    }
}
