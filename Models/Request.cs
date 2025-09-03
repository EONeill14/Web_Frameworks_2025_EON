using System.ComponentModel.DataAnnotations;

namespace Web_Frameworks_2025_EON.Models
{
    public enum RequestStatus { Pending, Accepted, Denied }

    public class Request
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public RequestStatus Status { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public string RequesterId { get; set; }
        public ApplicationUser Requester { get; set; }
    }
}