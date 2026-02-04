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

        // =========================
        // INDEX (SEARCH + SORT + PAGINATION)
        // =========================
        public async Task<IActionResult> Index(string? q, string? sort, int page = 1)
        {
            const int pageSize = 4;
            if (page < 1) page = 1;

            var query = _context.Tournaments
                .Include(t => t.Organizator)
                .Include(t => t.Registrations)
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(t =>
                    t.Naziv.Contains(term) ||
                    t.Sport.Contains(term)
                );
            }

            // SORT
            query = sort switch
            {
                "date_asc" => query.OrderBy(t => t.Datum).ThenBy(t => t.Vrijeme),
                "date_desc" => query.OrderByDescending(t => t.Datum).ThenByDescending(t => t.Vrijeme),
                "players_desc" => query.OrderByDescending(t => t.Registrations.Count),
                _ => query.OrderByDescending(t => t.CreatedAt)
            };

            // PAGINATION metadata
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var tournaments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            ViewBag.Q = q ?? "";
            ViewBag.Sort = sort ?? "";
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = pageSize;

            return View(viewModels);
        }

        // =========================
        // CREATE
        // =========================
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(TournamentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

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

            _context.Tournaments.Add(tournament);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Turnir je kreiran.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT
        // =========================
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null) return NotFound();

            var vm = new TournamentViewModel
            {
                Id = tournament.Id,
                Naziv = tournament.Naziv,
                Sport = tournament.Sport,
                BrojIgraca = tournament.BrojIgraca,
                Datum = tournament.Datum,
                Vrijeme = tournament.Vrijeme
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, TournamentViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null) return NotFound();

            tournament.Naziv = model.Naziv;
            tournament.Sport = model.Sport;
            tournament.BrojIgraca = model.BrojIgraca;
            tournament.Datum = model.Datum;
            tournament.Vrijeme = model.Vrijeme;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Turnir je ažuriran.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tournament = await _context.Tournaments
                .Include(t => t.Registrations)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null) return NotFound();

            var vm = new TournamentViewModel
            {
                Id = tournament.Id,
                Naziv = tournament.Naziv,
                Sport = tournament.Sport,
                BrojIgraca = tournament.BrojIgraca,
                Datum = tournament.Datum,
                Vrijeme = tournament.Vrijeme,
                BrojPrijavljenih = tournament.Registrations.Count
            };

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Registrations)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null) return NotFound();

            _context.Tournaments.Remove(tournament);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Turnir je obrisan.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // REGISTER / UNREGISTER
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Register(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var tournament = await _context.Tournaments
                .Include(t => t.Registrations)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
                return NotFound();

            if (tournament.JePun)
            {
                TempData["Error"] = "Turnir je pun.";
                return RedirectToAction(nameof(Index));
            }

            var exists = await _context.TournamentRegistrations
                .AnyAsync(r => r.TournamentId == id && r.UserId == userId);

            if (exists)
            {
                TempData["Error"] = "Već ste prijavljeni.";
                return RedirectToAction(nameof(Index));
            }

            _context.TournamentRegistrations.Add(new TournamentRegistration
            {
                TournamentId = id,
                UserId = userId,
                DatumPrijave = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Uspješno ste se prijavili.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Unregister(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var registration = await _context.TournamentRegistrations
                .FirstOrDefaultAsync(r => r.TournamentId == id && r.UserId == userId);

            if (registration == null)
            {
                TempData["Error"] = "Niste prijavljeni.";
                return RedirectToAction(nameof(Index));
            }

            _context.TournamentRegistrations.Remove(registration);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Odjava uspješna.";
            return RedirectToAction(nameof(Index));
        }
    }
}
