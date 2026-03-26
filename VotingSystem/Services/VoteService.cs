using Microsoft.EntityFrameworkCore;
using VotingSystem.Data;
using VotingSystem.Models;

namespace VotingSystem.Services
{
    public class VoteService
    {
        private readonly ApplicationContext _db;

        public VoteService(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<bool> HasVotedAsync(int pollId, string userId)
        {
            return await _db.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId);
        }

        public async Task<Vote?> CastVoteAsync(int pollId, int optionId, string userId)
        {
            if (await HasVotedAsync(pollId, userId))
                return null;

            var vote = new Vote
            {
                PollId = pollId,
                PollOptionId = optionId,
                UserId = userId
            };
            _db.Votes.Add(vote);
            await _db.SaveChangesAsync();
            return vote;
        }
    }
}
