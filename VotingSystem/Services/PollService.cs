using Microsoft.EntityFrameworkCore;
using VotingSystem.Data;
using VotingSystem.Models;

namespace VotingSystem.Services
{
    public class PollService
    {
        private readonly ApplicationContext _db;

        public PollService(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<List<Poll>> GetAllPollsAsync()
        {
            return await _db.Polls
                .Include(p => p.Options)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Poll?> GetPollAsync(int id)
        {
            return await _db.Polls
                .Include(p => p.Options)
                .Include(p => p.Votes)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Poll> CreatePollAsync(string name, List<string> options)
        {
            var poll = new Poll
            {
                Name = name,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Options = options.Select(o => new PollOption { Text = o }).ToList()
            };
            _db.Polls.Add(poll);
            await _db.SaveChangesAsync();
            return poll;
        }

        public async Task ToggleActiveAsync(int pollId)
        {
            var poll = await _db.Polls.FindAsync(pollId);
            if (poll != null)
            {
                poll.IsActive = !poll.IsActive;
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeletePollAsync(int pollId)
        {
            var poll = await _db.Polls.FindAsync(pollId);
            if (poll != null)
            {
                _db.Polls.Remove(poll);
                await _db.SaveChangesAsync();
            }
        }
    }
}
