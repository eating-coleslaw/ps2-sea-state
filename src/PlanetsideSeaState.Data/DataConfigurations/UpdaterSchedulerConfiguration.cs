// Credit to Lampjaw

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanetsideSeaState.Data.Models;

namespace PlanetsideSeaState.Data.DataConfigurations
{
    class UpdaterSchedulerConfiguration : IEntityTypeConfiguration<UpdaterScheduler>
    {
        public void Configure(EntityTypeBuilder<UpdaterScheduler> builder)
        {
            builder.ToTable("UpdaterScheduler");

            builder.HasKey(a => a.Id);
        }
    }
}
