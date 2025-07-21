using Microsoft.AspNetCore.SignalR;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PRN_Final_Project.Hubs
{
    public class ChatHub : Hub
    {
        // Join user's personal channel
        public async Task JoinUserChannel(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        // Send message to recipients
        public async Task SendMessage(JsonElement payload)
        {
            var message = payload.GetProperty("message");
            var recipients = payload.GetProperty("recipients");

            foreach (var recipient in recipients.EnumerateArray())
            {
                var recipientId = recipient.GetInt32(); // Use GetInt64() if long
                await Clients.Group($"user_{recipientId}").SendAsync("ReceiveMessage", message);
            }
        }



        // Send notification to recipients
        public async Task SendNotification(object notification)
        {
            dynamic notif = notification;
            var recipients = notif.recipientIds;

            foreach (var recipient in recipients)
            {
                await Clients.Group($"user_{recipient}").SendAsync("ReceiveNotification", notification);
            }
        }

        // Handle user added to conversation
        public async Task UserAdded(object payload)
        {
            dynamic data = payload;
            var recipients = data.recipients;

            foreach (var recipient in recipients)
            {
                await Clients.Group($"user_{recipient}").SendAsync("UserAdded", payload);
            }
        }

        // Handle user removed from conversation
        public async Task UserRemoved(object payload)
        {
            dynamic data = payload;
            var recipients = data.recipients;

            foreach (var recipient in recipients)
            {
                await Clients.Group($"user_{recipient}").SendAsync("UserRemoved", payload);
            }
        }
    }
}