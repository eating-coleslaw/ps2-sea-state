// Credit to Lampjaw

using static PlanetsideSeaState.Data.DbContextHelper;

namespace PlanetsideSeaState.Data
{
    public interface IDbContextHelper
    {
        DbContextFactory GetFactory();
    }
}
