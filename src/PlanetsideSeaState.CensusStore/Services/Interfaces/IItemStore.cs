using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore.Services
{
    public interface IItemStore : IUpdateable
    {
        Task<Item> GetItemByIdAsync(int itemId);
        Task<IEnumerable<ItemCategory>> GetAllWeaponItemCategoriesAsync();
        Task<IEnumerable<Item>> GetItemsByCategoryIdsAsync(params int[] categoryIds);
        Task<IEnumerable<Item>> GetItemsByIdsAsync(params int[] itemIds);
    }
}
