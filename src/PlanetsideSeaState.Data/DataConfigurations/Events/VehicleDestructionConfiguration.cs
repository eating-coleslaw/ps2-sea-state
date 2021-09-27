using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Events;

namespace PlanetsideSeaState.Data.DataConfigurations.Events
{
    public class VehicleDestructionConfiguration : IEntityTypeConfiguration<VehicleDestruction>
    {
        public void Configure(EntityTypeBuilder<VehicleDestruction> builder)
        {
            builder.ToTable("VehicleDestruction");

            builder.HasKey(e => new
            {
                e.Timestamp,
                e.AttackerCharacterId,
                e.VictimCharacterId,
                e.VictimVehicleId
            });

            builder.Ignore(e => e.AttackerCharacter);
            builder.Ignore(e => e.VictimCharacter);
            builder.Ignore(e => e.Weapon);
            builder.Ignore(e => e.AttackerVehicle);
            builder.Ignore(e => e.VictimVehicle);
            builder.Ignore(e => e.World);
            builder.Ignore(e => e.Zone);
            builder.Ignore(e => e.Facility);
        }
    }
}