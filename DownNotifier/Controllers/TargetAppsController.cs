using AspNetCoreHero.ToastNotification.Abstractions;
using DownNotifier.Data;
using DownNotifier.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace DownNotifier.Controllers
{
    public class TargetAppsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;

        public TargetAppsController(ApplicationDbContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.TargetApps.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Url,Interval")] TargetApp targetApp)
        {
            if (ModelState.IsValid)
            {
                _context.Add(targetApp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            _notyf.Success("Success Create");
            return View(targetApp);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var targetApp = await _context.TargetApps.FindAsync(id);
            if (targetApp == null)
            {
                return NotFound();
            }
            return View(targetApp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Url,Interval")] TargetApp targetApp)
        {
            if (id != targetApp.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(targetApp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TargetAppExists(targetApp.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _notyf.Success("Success Update");
            return View(targetApp);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var targetApp = await _context.TargetApps .FirstOrDefaultAsync(m => m.Id == id);
            if (targetApp == null)
            {
                _notyf.Error("Error Delete");
                return NotFound();
            }
            else
            {
                var notifications = await _context.Notifications.ToListAsync();
                _context.RemoveRange(notifications);
                _context.Remove(targetApp);
                await _context.SaveChangesAsync();
                _notyf.Success("Success Delete");
                return RedirectToAction(nameof(Index));
            }

        }

       

        private bool TargetAppExists(int id)
        {
            return _context.TargetApps.Any(e => e.Id == id);
        }
    }
}
