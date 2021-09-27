using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public VehicleRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int vehicleId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Vehicles.Where(v => v.Id == vehicleId).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Vehicles.ToListAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<Vehicle> entities)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            await dbContext.Vehicles.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

            await dbContext.SaveChangesAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<VehicleFaction> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}
