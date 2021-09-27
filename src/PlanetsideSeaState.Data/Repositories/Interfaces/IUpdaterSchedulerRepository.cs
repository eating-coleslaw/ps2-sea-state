using PlanetsideSeaState.Data.Models;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public interface IUpdaterSchedulerRepository
    {
        UpdaterScheduler GetUpdaterHistoryByServiceName(string serviceName);
        Task UpsertAsync(UpdaterScheduler entity);
    }
}
