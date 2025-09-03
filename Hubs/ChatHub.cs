using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Web_Frameworks_2025_EON.Data;
using Web_Frameworks_2025_EON.Models;

namespace Web_Frameworks_2025_EON.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string receiverId, string messageContent)
        {
            var senderId = Context.UserIdentifier;

            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId) || string.IsNullOrEmpty(messageContent))
            {
                return;
            }

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = messageContent,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Send the message to the specific receiver
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, messageContent);

            // ALSO send the message back to the person who sent it
            await Clients.Caller.SendAsync("ReceiveMessage", senderId, messageContent);
        }
    }
}