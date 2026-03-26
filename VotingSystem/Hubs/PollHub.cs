using Microsoft.AspNetCore.SignalR;
using VotingSystem.Services;

namespace VotingSystem.Hubs
{
    public class PollHub : Hub
    {
        private readonly ResultService _resultService;

        public PollHub(ResultService resultService)
        {
            _resultService = resultService;
        }

        public async Task JoinPoll(int pollId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"poll_{pollId}");
            var results = await _resultService.GetResultsAsync(pollId);
            await Clients.Caller.SendAsync("ReceiveResults", results);
        }

        public async Task LeavePoll(int pollId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"poll_{pollId}");
        }
    }
}
