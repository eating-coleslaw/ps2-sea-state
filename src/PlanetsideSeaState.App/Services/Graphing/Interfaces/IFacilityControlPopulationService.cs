using PlanetsideSeaState.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Graphing
{
    public interface IFacilityControlPopulationService
    {
        Task<FacilityControlPopulations> GetFacilityControlPopulationsAsync(Guid facilityControlId);
        Task<FacilityControlPopulations> GetFacilityControlPopulationsAsync(DateTime timestamp, int facilityId, short worldId);
    }
}
