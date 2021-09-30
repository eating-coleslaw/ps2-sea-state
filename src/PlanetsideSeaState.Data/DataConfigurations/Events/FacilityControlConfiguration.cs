using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Events;

namespace PlanetsideSeaState.Data.DataConfigurations.Events
{
    public class FacilityControlConfiguration : IEntityTypeConfiguration<FacilityControl>
    {
        public void Configure(EntityTypeBuilder<FacilityControl> builder)
        {
            builder.ToTable("FacilityControl");

            builder.HasKey(e => new
            {
                e.Timestamp,
                e.FacilityId,
                e.WorldId,
            });
        }
    }
}
