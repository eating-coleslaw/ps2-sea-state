﻿using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public ItemRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<Item> GetItemByIdAsync(int itemId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Items.Where(i => itemId == i.Id).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsByIdsAsync(params int[] itemIds)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Items.Where(i => itemIds.Contains(i.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryIdsAsync(params int[] categoryIds)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Items.Where(i => i.ItemCategoryId.HasValue && categoryIds.Contains(i.ItemCategoryId.Value)).ToListAsync();
        }

        public async Task<IEnumerable<ItemCategory>> GetAllWeaponItemCategoriesAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.ItemCategories.Where(e => e.IsWeaponCategory).ToListAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<Item> entities)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            await dbContext.Items.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

            await dbContext.SaveChangesAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<ItemCategory> entities)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            await dbContext.ItemCategories.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

            await dbContext.SaveChangesAsync();
        }
    }
}
