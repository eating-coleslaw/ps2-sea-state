using PlanetsideSeaState.Data.Models.Census;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore.Services
{
    public interface IMetagameEventStore : IUpdateable
    {
        Task<MetagameEventCategory> GetMetagameEventCategoryAsync(int metagameEventId);
    }
}
