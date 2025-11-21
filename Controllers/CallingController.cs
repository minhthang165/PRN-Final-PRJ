using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PRN_Final_Project.Hubs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Final_Project.Controllers
{
    [Route("calls")]
    [ApiController]
    public class CallingController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public CallingController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("create/{callerId}/{receiverId}/{roomID}")]
        public async Task<ActionResult<Dictionary<string, string>>> CreateCall(
            int callerId,
            int receiverId,
            int roomID)
        {
            Console.WriteLine($"Creating call: Caller {callerId} -> Receiver {receiverId} in Room {roomID}");

            var callData = new
            {
                callerId = callerId.ToString(),
                roomID = roomID.ToString()
            };

            // Send call notification to the receiver via SignalR
            await _hubContext.Clients.Group($"user_{receiverId}")
                .SendAsync("IncomingCall", callData);

            Console.WriteLine($"✅ Sent call notification to receiver: {receiverId}");

            return Ok(new Dictionary<string, string>
            {
                { "roomID", roomID.ToString() }
            });
        }

        [HttpPost("accept/{callerId}/{receiverId}/{roomID}")]
        public async Task<ActionResult> AcceptCall(
            int callerId,
            int receiverId,
            int roomID)
        {
            Console.WriteLine($"Call accepted: Receiver {receiverId} accepted call from {callerId} in Room {roomID}");

            var callAcceptedData = new
            {
                receiverId = receiverId.ToString(),
                roomID = roomID.ToString(),
                accepted = true
            };

            // Notify the caller that call was accepted
            await _hubContext.Clients.Group($"user_{callerId}")
                .SendAsync("CallAccepted", callAcceptedData);

            return Ok(new { message = "Call accepted successfully" });
        }

        [HttpPost("reject/{callerId}/{receiverId}/{roomID}")]
        public async Task<ActionResult> RejectCall(
            int callerId,
            int receiverId,
            int roomID)
        {
            Console.WriteLine($"Call rejected: Receiver {receiverId} rejected call from {callerId} in Room {roomID}");

            var callRejectedData = new
            {
                receiverId = receiverId.ToString(),
                roomID = roomID.ToString(),
                rejected = true
            };

            // Notify the caller that call was rejected
            await _hubContext.Clients.Group($"user_{callerId}")
                .SendAsync("CallRejected", callRejectedData);

            return Ok(new { message = "Call rejected successfully" });
        }

        [HttpPost("end/{callerId}/{receiverId}/{roomID}")]
        public async Task<ActionResult> EndCall(
            int callerId,
            int receiverId,
            int roomID)
        {
            Console.WriteLine($"Call ended in Room {roomID}");

            var callEndedData = new
            {
                roomID = roomID.ToString(),
                ended = true
            };

            // Notify both participants that the call ended
            await _hubContext.Clients.Group($"user_{callerId}")
                .SendAsync("CallEnded", callEndedData);

            await _hubContext.Clients.Group($"user_{receiverId}")
                .SendAsync("CallEnded", callEndedData);

            return Ok(new { message = "Call ended successfully" });
        }
    }
}