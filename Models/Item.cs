using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Frameworks_2025_EON.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public DateTime DatePosted { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string OwnerId { get; set; } = string.Empty;
        [ForeignKey("OwnerId")]
        public ApplicationUser? Owner { get; set; }

        public bool IsApproved { get; set; }

    }
}
