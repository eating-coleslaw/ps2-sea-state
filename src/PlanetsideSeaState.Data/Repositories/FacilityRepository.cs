using PlanetsideSeaState.Data.Models.Census;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public class FacilityRepository : IFacilityRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public FacilityRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityTypeAsync(int facilityTypeId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.MapRegions.Where(r => r.FacilityTypeId == facilityTypeId && r.IsCurrent).ToListAsync();
        }

        public async Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(int zoneId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.MapRegions.Where(r => r.ZoneId == zoneId && r.IsCurrent).ToListAsync();
        }

        public async Task<IEnumerable<MapRegion>> GetAllMapRegionsAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.MapRegions.Where(r => r.IsCurrent).ToListAsync();
        }

        public async Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityIdsAsync(IEnumerable<int> facilityIds)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.MapRegions.Where(r => facilityIds.Contains(r.FacilityId) && r.IsCurrent).ToListAsync();
        }

        public async Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(int zoneId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.FacilityLinks.Where(l => l.ZoneId == zoneId).ToListAsync();
        }

        public async Task<IEnumerable<FacilityLink>> GetFacilityLinksByFacilityIdsAsync(IEnumerable<int> facilityIds)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.FacilityLinks.Where(l => facilityIds.Contains(l.FacilityIdA) || facilityIds.Contains(l.FacilityIdB)).Distinct().ToListAsync();
        }

        //public async Task UpsertRangeAsync(IEnumerable<MapRegion> entities)
        //{
        //    using var factory = _dbContextHelper.GetFactory();
        //    var dbContext = factory.GetDbContext();

        //    await dbContext.MapRegions.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id && a.FacilityId == e.FacilityId);

        //    await dbContext.SaveChangesAsync();
        //}

        public async Task UpsertRangeAsync(IEnumerable<MapRegion> censusEntities)
        {
            var createdEntities = new List<MapRegion>();

            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            //var storeEntities = await GetAllStoreMapRegionsAsync();
            var storeEntities = await dbContext.MapRegions.ToListAsync();

            var allEntities = new List<MapRegion>(censusEntities.Select(e => new MapRegion() { Id = e.Id, FacilityId = e.FacilityId }));

            allEntities.AddRange(storeEntities
                                    .Where(s => !allEntities.Any(c => c.Id == s.Id && c.FacilityId == s.FacilityId))
                                    .Select(e => new MapRegion() { Id = e.Id, FacilityId = e.FacilityId }));

            foreach (var entity in allEntities)
            {
                var censusEntity = censusEntities.FirstOrDefault(e => e.Id == entity.Id && e.FacilityId == entity.FacilityId);
                var censusMapRegion = censusEntities.FirstOrDefault(e => e.Id == entity.Id);

                var storeEntity = storeEntities.FirstOrDefault(e => e.Id == entity.Id && e.FacilityId == entity.FacilityId);
                var storeMapRegion = storeEntities.FirstOrDefault(e => e.Id == entity.Id);

                if (censusEntity != null)
                {
                    if (storeEntity == null)
                    {
                        // Brand New MapRegion
                        if (storeMapRegion == null)
                        {
                            censusEntity.IsDeprecated = false;
                            censusEntity.IsCurrent = true;

                            createdEntities.Add(censusEntity);
                        }
                        // Existing MapRegion overwritten with new FacilityID
                        else if (censusEntity.FacilityId != 0)
                        {
                            censusEntity.IsDeprecated = false;
                            censusEntity.IsCurrent = true;

                            createdEntities.Add(censusEntity);

                            storeEntity = storeMapRegion;
                            storeEntity.IsDeprecated = true;
                            storeEntity.IsCurrent = false;

                            dbContext.MapRegions.Update(storeEntity);
                        }
                        // Existing MapRegion is Deleted with no replacement, but still shows up in the Census API
                        else
                        {
                            storeEntity = storeMapRegion;
                            storeEntity.IsDeprecated = true;
                            storeEntity.IsCurrent = true;

                            dbContext.MapRegions.Update(storeEntity);
                        }
                    }
                    // Existing MapRegion updated somehow
                    else
                    {
                        storeEntity = censusEntity;

                        storeEntity.IsDeprecated = false;
                        storeEntity.IsCurrent = true;

                        dbContext.MapRegions.Update(storeEntity);
                    }
                }
                // Existing MapRegion is Deleted with no replacement, and doesn't show up in the Census API
                else
                {
                    if (storeEntity != null)
                    {
                        storeEntity.IsDeprecated = true;
                        storeEntity.IsCurrent = (censusMapRegion != null && censusMapRegion.Id == storeEntity.Id) ? false : true;

                        dbContext.MapRegions.Update(storeEntity);
                    }
                }
            }

            if (createdEntities.Any())
            {
                dbContext.MapRegions.AddRange(createdEntities);
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<FacilityLink> entities)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var storeEntities = await dbContext.FacilityLinks.ToListAsync();

            foreach (var entity in entities)
            {
                var storeEntity = storeEntities.SingleOrDefault(e => e.FacilityIdA == entity.FacilityIdA && e.FacilityIdB == entity.FacilityIdB && e.ZoneId == entity.ZoneId);

                if (storeEntity == null)
                {
                    dbContext.FacilityLinks.Add(entity);
                }
                else
                {
                    entity.Id = storeEntity.Id;
                    storeEntity = entity;
                    dbContext.FacilityLinks.Update(storeEntity);
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task UpserRangeAsync(IEnumerable<FacilityType> entities)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            await dbContext.FacilityTypes.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

            await dbContext.SaveChangesAsync();
        }
    }
}
