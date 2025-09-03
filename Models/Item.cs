using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Frameworks_2025_EON.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please specify the listing type")]
        public string ListingType { get; set; } = string.Empty;

        public string? Brand { get; set; }
        public string? Condition { get; set; }

        public DateTime DatePosted { get; set; }
        public bool IsApproved { get; set; }

        [Required(ErrorMessage = "Please specify a category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string OwnerId { get; set; } = string.Empty;
        [ForeignKey("OwnerId")]
        public ApplicationUser? Owner { get; set; }
    }
}