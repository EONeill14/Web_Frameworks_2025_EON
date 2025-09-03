using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_Frameworks_2025_EON.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}