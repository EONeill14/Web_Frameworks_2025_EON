using Web_Frameworks_2025_EON.Models;

namespace Web_Frameworks_2025_EON.Repositories
{
    // In Repositories/IItemRepository.cs
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllApprovedAsync();
        Task<IEnumerable<Item>> GetAllPendingAsync();
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Item?> GetByIdAsync(int id);
        Task AddAsync(Item item);
        Task UpdateAsync(Item item);
        Task DeleteAsync(Item item);
        Task<bool> ExistsAsync(int id);
    }
}