using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using VotingSystem.Hubs;
using VotingSystem.Models;
using VotingSystem.Services;

namespace VotingSystem.Controllers
{
    public class PollController : Controller
    {
        private readonly PollService _pollService;
        private readonly VoteService _voteService;
        private readonly ResultService _resultService;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<PollHub> _hubContext;

        public PollController(PollService pollService, VoteService voteService,
            ResultService resultService, UserManager<User> userManager,
            IHubContext<PollHub> hubContext)
        {
            _pollService = pollService;
            _voteService = voteService;
            _resultService = resultService;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var polls = await _pollService.GetAllPollsAsync();
            return View(polls);
        }

        public async Task<IActionResult> Details(int id)
        {
            var poll = await _pollService.GetPollAsync(id);
            if (poll == null) return NotFound();

            bool hasVoted = false;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User)!;
                hasVoted = await _voteService.HasVotedAsync(id, userId);
            }

            ViewBag.HasVoted = hasVoted;
            return View(poll);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int pollId, int optionId)
        {
            var userId = _userManager.GetUserId(User)!;
            var vote = await _voteService.CastVoteAsync(pollId, optionId, userId);

            if (vote != null)
            {
                var results = await _resultService.GetResultsAsync(pollId);
                await _hubContext.Clients.Group($"poll_{pollId}").SendAsync("ReceiveResults", results);
            }

            return RedirectToAction("Details", new { id = pollId });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, List<string> options)
        {
            if (string.IsNullOrWhiteSpace(name) || options == null || options.Count(o => !string.IsNullOrWhiteSpace(o)) < 2)
            {
                ModelState.AddModelError("", "Введите название и минимум 2 варианта ответа.");
                return View();
            }

            var filtered = options.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
            await _pollService.CreatePollAsync(name, filtered);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            await _pollService.ToggleActiveAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _pollService.DeletePollAsync(id);
            return RedirectToAction("Index");
        }
    }
}
