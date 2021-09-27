using PlanetsideSeaState.CensusServices;
using PlanetsideSeaState.CensusServices.Models;
using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore.Services
{
    public class ItemStore : IItemStore
    {
        private readonly IItemRepository _itemRepository;
        private readonly CensusItem _censusItem;
        private readonly CensusItemCategory _censusItemCategory;
        private readonly ILogger<ItemStore> _logger;

        public string StoreName => "ItemStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(14);

        private static readonly List<int> _nonWeaponItemCategoryIds = new List<int>()
        {
            99,  // Camo
            101, // Vehicles
            103, // Infantry Gear
            105, // Vehicle Gear
            106, // Armor Camo
            107, // Weapon Camo
            108, // Vehicle Camo
            133, // Implants
            134, // Consolidated Camo
            135, // VO Packs
            136, // Male VO Pack
            137, // Female VO Pack
            139, // Infantry Abilities
            140, // Vehicle Abilities
            141, // Boosts & Utilities
            142, // Consolidated Decal
            143, // Attachments
            145, // ANT Utility
            146, // Bounty Contract
            148  // ANT Harvesting Tool
        };


        public ItemStore(IItemRepository itemRepository, CensusItem censusItem, CensusItemCategory censusItemCategory, ILogger<ItemStore> logger)
        {
            _itemRepository = itemRepository;
            _censusItem = censusItem;
            _censusItemCategory = censusItemCategory;
            _logger = logger;
        }

        public async Task<Item> GetItemByIdAsync(int itemId)
        {
            return await _itemRepository.GetItemByIdAsync(itemId);
        }

        public async Task<IEnumerable<Item>> GetItemsByIdsAsync(params int[] itemIds)
        {
            return await _itemRepository.GetItemsByIdsAsync(itemIds);
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryIdsAsync(params int[] categoryIds)
        {
            return await _itemRepository.GetItemsByCategoryIdsAsync(categoryIds);
        }

        public async Task<IEnumerable<ItemCategory>> GetAllWeaponItemCategoriesAsync()
        {
            return await _itemRepository.GetAllWeaponItemCategoriesAsync();
        }

        public async Task RefreshStore()
        {
            var items = await _censusItem.GetAllWeaponItems();

            var itemCategories = await _censusItemCategory.GetAllItemCategories();

            if (items != null)
            {
                await _itemRepository.UpsertRangeAsync(items.Select(ConvertToDbModel));
            }

            if (itemCategories != null)
            {
                await _itemRepository.UpsertRangeAsync(itemCategories.Select(ConvertToDbModel));
            }
        }

        private static Item ConvertToDbModel(CensusItemModel item)
        {
            return new Item
            {
                Id = item.ItemId,
                ItemTypeId = item.ItemTypeId,
                ItemCategoryId = item.ItemCategoryId,
                IsVehicleWeapon = item.IsVehicleWeapon,
                Name = item.Name?.English,
                Description = item.Description?.English,
                FactionId = item.FactionId,
                MaxStackSize = item.MaxStackSize,
                ImageId = item.ImageId
            };
        }

        private static ItemCategory ConvertToDbModel(CensusItemCategoryModel itemCategory)
        {
            var id = itemCategory.ItemCategoryId;

            var isWeaponCategory = GetIsWeaponItemCategory(id);

            return new ItemCategory
            {
                Id = id,
                Name = itemCategory.Name.English,
                IsWeaponCategory = isWeaponCategory
            };
        }

        private static bool GetIsWeaponItemCategory(int itemCategoryId)
        {
            return !_nonWeaponItemCategoryIds.Contains(itemCategoryId);
        }
    }
}
