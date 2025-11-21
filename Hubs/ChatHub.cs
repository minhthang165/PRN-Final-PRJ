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

        // NEW: Handle incoming calls
        public async Task CreateCall(int callerId, int receiverId, int roomId)
        {
            Console.WriteLine($"Creating call: Caller {callerId} -> Receiver {receiverId} in Room {roomId}");

            var callData = new
            {
                callerId = callerId.ToString(),
                roomID = roomId.ToString()
            };

            // Send call notification to the receiver
            await Clients.Group($"user_{receiverId}").SendAsync("IncomingCall", callData);
            
            Console.WriteLine($"✅ Sent call notification to receiver: {receiverId}");
        }

        // Handle call acceptance
        public async Task AcceptCall(int callerId, int receiverId, int roomId)
        {
            var callAcceptedData = new
            {
                receiverId = receiverId.ToString(),
                roomID = roomId.ToString(),
                accepted = true
            };

            await Clients.Group($"user_{callerId}").SendAsync("CallAccepted", callAcceptedData);
        }

        // Handle call rejection
        public async Task RejectCall(int callerId, int receiverId, int roomId)
        {
            var callRejectedData = new
            {
                receiverId = receiverId.ToString(),
                roomID = roomId.ToString(),
                rejected = true
            };

            await Clients.Group($"user_{callerId}").SendAsync("CallRejected", callRejectedData);
        }
    }
}