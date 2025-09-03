using System.Collections.Generic;
using Web_Frameworks_2025_EON.Models;

namespace Web_Frameworks_2025_EON.Models.ViewModels
{
    public class ChatViewModel
    {
        public List<Message> Messages { get; set; } = new List<Message>();
        public string ReceiverId { get; set; } = string.Empty;
        public string ReceiverEmail { get; set; } = string.Empty;
    }
}