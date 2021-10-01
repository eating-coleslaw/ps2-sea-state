using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Events;

namespace PlanetsideSeaState.Data.DataConfigurations.Events
{
    public class PlayerFacilityControlConfiguration : IEntityTypeConfiguration<PlayerFacilityControl>
    {
        public void Configure(EntityTypeBuilder<PlayerFacilityControl> builder)
        {
            builder.ToTable("PlayerFacilityControl");

            builder.HasKey(e => new
            {
                e.Timestamp,
                e.FacilityId,
                e.CharacterId
            });

            builder.HasOne(player => player.FacilityControl)
                .WithMany(facility => facility.PlayerControls)
                .HasForeignKey(player => player.FacilityControlId);

            builder.Ignore(e => e.Character);
            builder.Ignore(e => e.Facility);
            //builder.Ignore(e => e.FacilityControl);
        }
    }
}
