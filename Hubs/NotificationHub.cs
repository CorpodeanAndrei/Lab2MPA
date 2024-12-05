using Microsoft.AspNetCore.SignalR;

namespace Lab2MPA.Hubs
{

    public class NotificationHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;

        public NotificationHub(PresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var result = await _presenceTracker.ConnectionOpened(Context.User.Identity.Name);
            if (result)
            {
                await Clients.All.SendAsync("newMessage", "system", $"{Context.User.Identity.Name} joined");
            }

            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("newMessage", "system", $"Currently online:\n{string.Join("\n", currentUsers)}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _presenceTracker.ConnectionClosed(Context.User.Identity.Name);
            await Clients.All.SendAsync("newMessage", "system", $"{Context.User.Identity.Name} left");
            await base.OnDisconnectedAsync(exception);
        }
    } 
}

