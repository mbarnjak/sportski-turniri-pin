using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turniri.Data;
using Turniri.Models;
using Turniri.ViewModels;

namespace Turniri.Controllers
{
    public class TournamentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TournamentController> _logger;

        public TournamentController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<TournamentController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Tournament
        public async Task<IActionResult> Index()
        {
            var tournaments = await _context.Tournaments
                .Include(t => t.Organizator)
                .Include(t => t.Registrations)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var userId = _userManager.GetUserId(User);
            var userRegistrations = userId != null
                ? await _context.TournamentRegistrations
                    .Where(r => r.UserId == userId)
                    .Select(r => r.TournamentId)
                    .ToListAsync()
                : new List<int>();

            var viewModels = tournaments.Select(t => new TournamentViewModel
            {
                Id = t.Id,
                Naziv = t.Naziv,
                Sport = t.Sport,
                BrojIgraca = t.BrojIgraca,
                Datum = t.Datum,
                Vrijeme = t.Vrijeme,
                OrganizatorIme = t.Organizator?.UserName ?? "Nepoznato",
                BrojPrijavljenih = t.BrojPrijavljenih,
                JePun = t.JePun,
                JePrijavljen = userRegistrations.Contains(t.Id)
            }).ToList();

            return View(viewModels);
        }

        // GET: Tournament/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tournament/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(TournamentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var tournament = new Tournament
                {
                    Naziv = model.Naziv,
                    Sport = model.Sport,
                    BrojIgraca = model.BrojIgraca,
                    Datum = model.Datum,
                    Vrijeme = model.Vrijeme,
                    OrganizatorId = userId,
                    CreatedAt = DateTime.Now
                };

                _context.Add(tournament);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tournament created: {TournamentId}", tournament.Id);

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // POST: Tournament/Register/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Register(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var tournament = await _context.Tournaments
                .Include(t => t.Registrations)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
            {
                return NotFound();
            }

            // Check if already registered
            var existingRegistration = await _context.TournamentRegistrations
                .FirstOrDefaultAsync(r => r.TournamentId == id && r.UserId == userId);

            if (existingRegistration != null)
            {
                TempData["Error"] = "Već ste prijavljeni na ovaj turnir.";
                return RedirectToAction(nameof(Index));
            }

            // Check if tournament is full
            if (tournament.JePun)
            {
                TempData["Error"] = "Turnir je pun.";
                return RedirectToAction(nameof(Index));
            }

            // Check if user is the organizer
            if (tournament.OrganizatorId == userId)
            {
                TempData["Error"] = "Ne možete se prijaviti na vlastiti turnir.";
                return RedirectToAction(nameof(Index));
            }

            var registration = new TournamentRegistration
            {
                TournamentId = id,
                UserId = userId,
                DatumPrijave = DateTime.Now
            };

            _context.Add(registration);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User {UserId} registered for tournament {TournamentId}", userId, id);

            TempData["Success"] = "Uspješno ste se prijavili na turnir!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Tournament/Unregister/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Unregister(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var registration = await _context.TournamentRegistrations
                .FirstOrDefaultAsync(r => r.TournamentId == id && r.UserId == userId);

            if (registration == null)
            {
                TempData["Error"] = "Niste prijavljeni na ovaj turnir.";
                return RedirectToAction(nameof(Index));
            }

            _context.TournamentRegistrations.Remove(registration);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User {UserId} unregistered from tournament {TournamentId}", userId, id);

            TempData["Success"] = "Uspješno ste se odjavili s turnira.";
            return RedirectToAction(nameof(Index));
        }
    }
}

