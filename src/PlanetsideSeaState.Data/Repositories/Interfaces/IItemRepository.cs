using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public interface IItemRepository
    {
        Task<Item> GetItemByIdAsync(int itemId);
        Task<IEnumerable<Item>> GetItemsByIdsAsync(params int[] itemIds);
        Task<IEnumerable<Item>> GetItemsByCategoryIdsAsync(params int[] categoryIds);
        Task<IEnumerable<ItemCategory>> GetAllWeaponItemCategoriesAsync();

        Task UpsertRangeAsync(IEnumerable<Item> entities);
        Task UpsertRangeAsync(IEnumerable<ItemCategory> entities);
    }
}
