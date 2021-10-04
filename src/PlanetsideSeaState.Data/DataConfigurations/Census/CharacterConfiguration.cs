using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanetsideSeaState.Data.Models.Census;

namespace PlanetsideSeaState.Data.DataConfigurations.Census
{
    public class CharacterConfiguration : IEntityTypeConfiguration<Character>
    {
        public void Configure(EntityTypeBuilder<Character> builder)
        {
            builder.ToTable("Character");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.PrestigeLevel).HasDefaultValue(0);

            builder.Ignore(e => e.IsOnline);
            builder.Ignore(e => e.OutfitMember);
        }
    }
}
