using dating_course_api.Src.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace dating_course_api.Src.SignalR
{
    [Authorize]
    public class PresenceHub(PresenceTracker tracker) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User == null)
                throw new HubException("Cannot get current user claim");

            var isOnline = await tracker.UserConnected(
                Context.User.GetUserId(),
                Context.ConnectionId
            );
            if (isOnline)
                await Clients.Others.SendAsync("UserIsOnline", Context.User?.GetUserId());

            var currentUsers = await tracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.User == null)
                throw new HubException("Cannot get current user claim");

            var userId = Context.User.GetUserId();
            var isOffline = await tracker.UserDisconnected(userId, Context.ConnectionId);

            if (isOffline)
            {
                await Clients.Others.SendAsync("UserIsOffline", userId);

                // Actualizar lista de usuarios online para todos
                var currentUsers = await tracker.GetOnlineUsers();
                await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendHeartbeat()
        {
            if (Context.User == null)
                return;

            await Clients.Caller.SendAsync("ReceiveHeartbeat");
        }
    }
}
