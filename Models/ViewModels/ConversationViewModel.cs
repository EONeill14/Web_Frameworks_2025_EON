using Web_Frameworks_2025_EON.Models;

namespace Web_Frameworks_2025_EON.Models.ViewModels
{
    public class ConversationViewModel
    {
        public string OtherUserId { get; set; }
        public string OtherUserEmail { get; set; }
        public Message LastMessage { get; set; }
    }
}