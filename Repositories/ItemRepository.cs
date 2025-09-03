using Microsoft.EntityFrameworkCore;
using Web_Frameworks_2025_EON.Data;
using Web_Frameworks_2025_EON.Models;

namespace Web_Frameworks_2025_EON.Repositories
{
    // In Repositories/ItemRepository.cs
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemRepository(ApplicationDbContext context) { _context = context; }

        public async Task<IEnumerable<Item>> GetAllApprovedAsync() => await _context.Items.Where(i => i.IsApproved).Include(i => i.Category).Include(i => i.Owner).ToListAsync();
        public async Task<IEnumerable<Item>> GetAllPendingAsync() => await _context.Items.Where(i => !i.IsApproved).Include(i => i.Category).Include(i => i.Owner).ToListAsync();
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync() => await _context.Categories.ToListAsync();
        public async Task<Item?> GetByIdAsync(int id) => await _context.Items.Include(i => i.Category).Include(i => i.Owner).FirstOrDefaultAsync(i => i.Id == id);
        public async Task AddAsync(Item item) { _context.Add(item); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(Item item) { _context.Update(item); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(Item item) { _context.Items.Remove(item); await _context.SaveChangesAsync(); }
        public async Task<bool> ExistsAsync(int id) => await _context.Items.AnyAsync(e => e.Id == id);
    }
}