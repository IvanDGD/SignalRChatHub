using Microsoft.AspNetCore.SignalR;
using WebApplication5.Models;

namespace WebApplication5.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, string> _users = new();
        private static readonly Dictionary<string, (string ChannelKey, string Owner)> _messages = new();

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var entry = _users.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (entry.Key != null)
            {
                _users.Remove(entry.Key);
                await Clients.Others.SendAsync("UserLeft", entry.Key);
                await Clients.All.SendAsync("Notify", $"{entry.Key} покинул чат");
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task Register(string userName)
        {
            _users[userName] = Context.ConnectionId;
            await Clients.Others.SendAsync("UserJoined", userName);
            await Clients.All.SendAsync("Notify", $"{userName} вошёл в чат");
        }

        public async Task Send(string text, string userName)
        {
            var msg = new ChatMessage
            {
                UserName = userName,
                Text = text,
                ChannelKey = "__public__"
            };
            _messages[msg.Id] = ("__public__", userName);
            await Clients.All.SendAsync("Receive", msg);
        }

        public async Task SendPrivate(string text, string fromUser, string toUser)
        {
            if (!_users.TryGetValue(toUser, out var connectionId))
            {
                await Clients.Caller.SendAsync("Notify", $"Пользователь «{toUser}» не в сети");
                return;
            }

            var pair = new[] { fromUser, toUser };
            Array.Sort(pair, StringComparer.Ordinal);
            var channelKey = $"dm_{pair[0]}_{pair[1]}";

            var msg = new ChatMessage
            {
                UserName = fromUser,
                Text = text,
                IsPrivate = true,
                ChannelKey = channelKey,
                ToUser = toUser
            };
            _messages[msg.Id] = (channelKey, fromUser);

            await Clients.Client(connectionId).SendAsync("ReceivePrivate", msg);
            await Clients.Caller.SendAsync("ReceivePrivate", msg);
        }

        public async Task DeleteMessage(string id, string channelKey)
        {
            if (!_messages.TryGetValue(id, out var meta))
                return;

            var callerUser = _users.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (callerUser == null || meta.Owner != callerUser)
                return;

            _messages.Remove(id);

            if (channelKey == "__public__")
            {
                await Clients.All.SendAsync("MessageDeleted", new { id, channelKey });
            }
            else
            {
                var parts = channelKey.Split('_', 3);
                if (parts.Length == 3)
                {
                    var ids = new List<string>();
                    foreach (var name in new[] { parts[1], parts[2] })
                        if (_users.TryGetValue(name, out var cid))
                            ids.Add(cid);
                    await Clients.Clients(ids).SendAsync("MessageDeleted", new { id, channelKey });
                }
            }
        }

        public Task<List<string>> GetOnlineUsers() => Task.FromResult(_users.Keys.ToList());
    }
}