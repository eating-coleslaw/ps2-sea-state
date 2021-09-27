using PlanetsideSeaState.CensusStore.Services;
using PlanetsideSeaState.Data.Models.Census;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public class ItemService : IItemService
    {
        private readonly IItemStore _itemStore;
        private readonly ILogger<ItemService> _logger;


        public ItemService(IItemStore itemStore, ILogger<ItemService> logger)
        {
            _itemStore = itemStore;
            _logger = logger;
        }


        public async Task<Item> GetItemByIdAsync(int itemId)
        {
            return await _itemStore.GetItemByIdAsync(itemId);
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryIdAsync(int categoryId)
        {
            return await _itemStore.GetItemsByCategoryIdsAsync(categoryId);
        }

        public async Task<IEnumerable<ItemCategory>> GetAllWeaponItemCategoriesAsync()
        {
            return await _itemStore.GetAllWeaponItemCategoriesAsync();
        }
    }
}
