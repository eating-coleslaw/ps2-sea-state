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

            builder.Property(e => e.Points).HasDefaultValue(0);

            builder.Ignore(e => e.Zone);
            builder.Ignore(e => e.World);
        }
    }
}
