using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Events;

namespace PlanetsideSeaState.Data.DataConfigurations.Events
{
    class PlayerLoginConfiguration : IEntityTypeConfiguration<PlayerLogin>
    {
        public void Configure(EntityTypeBuilder<PlayerLogin> builder)
        {
            builder.ToTable("PlayerLogin");

            builder.HasKey(e => new
            {
                e.Timestamp,
                e.CharacterId
            });

            builder.Ignore(e => e.Character);
        }
    }
}
