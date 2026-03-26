using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Data;
using VotingSystem.Models;

namespace VotingSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        public AdminController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var polls = await _context.Polls
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(polls);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Poll poll)
        {
            if (ModelState.IsValid)
            {
                poll.CreatedAt = DateTime.UtcNow;
                _context.Polls.Add(poll);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var polls = await _context.Polls
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View("Index", polls);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var polls = await _context.Polls.FindAsync(id);
            if (polls == null)
                return NotFound();


            _context.Polls.Remove(polls);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
