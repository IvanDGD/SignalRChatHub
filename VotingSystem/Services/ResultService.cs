using Microsoft.EntityFrameworkCore;
using VotingSystem.Data;

namespace VotingSystem.Services
{
    public class PollResult
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; } = "";
        public int Votes { get; set; }
        public double Percentage { get; set; }
    }

    public class ResultService
    {
        private readonly ApplicationContext _db;

        public ResultService(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<List<PollResult>> GetResultsAsync(int pollId)
        {
            var options = await _db.PollOptions
                .Where(o => o.PollId == pollId)
                .Select(o => new { o.Id, o.Text, VoteCount = o.Votes.Count })
                .ToListAsync();

            int total = options.Sum(o => o.VoteCount);

            return options.Select(o => new PollResult
            {
                OptionId = o.Id,
                OptionText = o.Text,
                Votes = o.VoteCount,
                Percentage = total == 0 ? 0 : Math.Round((double)o.VoteCount / total * 100, 1)
            }).ToList();
        }
    }
}
