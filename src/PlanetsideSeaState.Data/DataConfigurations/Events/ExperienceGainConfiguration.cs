using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Events;

namespace PlanetsideSeaState.Data.DataConfigurations.Events
{
    public class ExperienceGainConfiguration : IEntityTypeConfiguration<ExperienceGain>
    {
        public void Configure(EntityTypeBuilder<ExperienceGain> builder)
        {
            builder.ToTable("ExperienceGain");

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.Timestamp, e.WorldId, e.ZoneId });
            //builder.HasIndex(a => new { a.Timestamp, a.WorldId, a.ExperienceId, a.ZoneId });
            //builder.HasIndex(a => new { a.Timestamp, a.CharacterId, a.ExperienceId });

            builder.Ignore(e => e.ActingCharacter);
            builder.Ignore(e => e.RecipientCharacter);
        }
    }
}
