using PlanetsideSeaState.Data.Models.Census;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PlanetsideSeaState.Data.DataConfigurations.Census
{
    public class FacilityLinkConfiguration : IEntityTypeConfiguration<FacilityLink>
    {
        public void Configure(EntityTypeBuilder<FacilityLink> builder)
        {
            builder.ToTable("FacilityLink");

            builder.HasKey(e => e.Id);

            builder.Ignore(e => e.FacilityA);
            builder.Ignore(e => e.FacilityB);
        }
    }
}
