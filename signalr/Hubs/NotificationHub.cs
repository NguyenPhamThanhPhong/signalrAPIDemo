using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using signalr.Model;
using System.Security.Claims;
using System.Text.Json;

namespace signalr.Hubs
{
    public class NotificationHub : Hub
    {
        private static Dictionary<string, string> _connectedUser = new Dictionary<string, string>();
        [Authorize]
        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                Console.WriteLine(userId);
                _connectedUser.Add(userId, Context.ConnectionId);
            }
            return base.OnConnectedAsync();
        }
        [Authorize]
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                _connectedUser.Remove(userId);
            }
            return base.OnDisconnectedAsync(exception);
        }
        [Authorize]
        public async Task SendNotification(List<string> Receivers, Notification notification)
        {
            Console.WriteLine(JsonSerializer.Serialize(Receivers));
            Console.WriteLine(JsonSerializer.Serialize(_connectedUser));
            List<string> connectionIds = new List<string>();
            foreach (var receiver in Receivers)
            {
                if (_connectedUser.TryGetValue(receiver, out var chosenConnectionId))
                {
                    connectionIds.Add(chosenConnectionId);
                }
            }
            Task[] notificationTasks = connectionIds.Select(
                connectionId => Clients.Client(connectionId).
                SendAsync("ReceiveNotification", notification)).ToArray();
            await Task.WhenAll(notificationTasks);
        }
    }
}
