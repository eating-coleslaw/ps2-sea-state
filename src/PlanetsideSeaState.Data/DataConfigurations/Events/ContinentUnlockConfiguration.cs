using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Events;

namespace PlanetsideSeaState.Data.DataConfigurations.Events
{
    public class ContinentUnlockConfiguration : IEntityTypeConfiguration<ContinentUnlock>
    {
        public void Configure(EntityTypeBuilder<ContinentUnlock> builder)
        {
            builder.ToTable("ContinentUnlock");

            builder.HasKey(e => new
            {
                e.Timestamp,
                e.WorldId,
                e.ZoneId
            });
        }
    }
}
