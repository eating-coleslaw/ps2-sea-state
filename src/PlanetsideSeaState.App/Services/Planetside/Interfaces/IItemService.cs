using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public interface IItemService
    {
        Task<IEnumerable<ItemCategory>> GetAllWeaponItemCategoriesAsync();
        Task<Item> GetItemByIdAsync(int itemId);
        Task<IEnumerable<Item>> GetItemsByCategoryIdAsync(int categoryId);
    }
}