using Microsoft.AspNetCore.Identity;

namespace Web_Frameworks_2025_EON.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
