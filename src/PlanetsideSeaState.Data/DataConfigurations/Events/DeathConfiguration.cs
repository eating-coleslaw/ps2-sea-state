using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Events;

namespace PlanetsideSeaState.Data.DataConfigurations.Events
{
    public class DeathConfiguration : IEntityTypeConfiguration<Death>
    {
        public void Configure(EntityTypeBuilder<Death> builder)
        {
            builder.ToTable("Death");

            builder.HasKey(e => new
            {
                e.Timestamp,
                e.AttackerCharacterId,
                e.VictimCharacterId
            });

            builder.Ignore(e => e.AttackerCharacter);
            builder.Ignore(e => e.VictimCharacter);
            builder.Ignore(e => e.Weapon);
            builder.Ignore(e => e.AttackerVehicle);
            builder.Ignore(e => e.World);
            builder.Ignore(e => e.Zone);
        }
    }
}
