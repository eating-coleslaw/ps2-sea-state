using PlanetsideSeaState.Data.DataConfigurations.Census;
using PlanetsideSeaState.Data.DataConfigurations.Events;
using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Data.Models.Events;
using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.DataConfigurations;
using PlanetsideSeaState.Data.Models;

namespace PlanetsideSeaState.Data
{
    public class PlanetmansDbContext : DbContext
    {
        public PlanetmansDbContext(DbContextOptions<PlanetmansDbContext> options)
            : base(options)
        {
        }

        #region Event DbSets
        public DbSet<Character> Characters { get; set; }
        //public DbSet<ContinentLock> ContinentLocks { get; set; }
        //public DbSet<ContinentUnlock> ContinentUnlocks { get; set; }
        public DbSet<Death> Deaths { get; set; }
        public DbSet<ExperienceGain> ExperienceGains { get; set; }
        public DbSet<FacilityControl> FacilityControls { get; set; }
        //public DbSet<MetagameEvent> MetagameEvents { get; set; }
        public DbSet<PlayerFacilityCapture> PlayerFacilityCaptures { get; set; }
        public DbSet<PlayerFacilityDefend> PlayerFacilityDefenses { get; set; }
        public DbSet<PlayerLogin> PlayerLogins { get; set; }
        public DbSet<PlayerLogout> PlayerLogouts { get; set; }
        public DbSet<VehicleDestruction> VehicleDestructions { get; set; }
        #endregion Event DbSets

        #region Census DbSets
        public DbSet<UpdaterScheduler> UpdaterScheduler { get; set; }

        //public DbSet<Experience> Experiences{ get; set; }
        //public DbSet<Faction> Factions { get; set; }
        public DbSet<FacilityLink> FacilityLinks{ get; set; }
        public DbSet<FacilityType> FacilityTypes { get; set; }
        //public DbSet<Item> Items { get; set; }
        //public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<Loadout> Loadouts { get; set; }
        public DbSet<MapRegion> MapRegions { get; set; }
        //public DbSet<MetagameEventCategory> MetagameEventCategories { get; set; }
        //public DbSet<MetagameEventState> MetagameEventStates { get; set; }
        //public DbSet<Profile> Profiles { get; set; }
        //public DbSet<Vehicle> Vehicles { get; set; }
        //public DbSet<World> Worlds { get; set; }
        //public DbSet<Zone> Zones { get; set; }
        #endregion Census DbSets

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Event Data Configurations
            //builder.ApplyConfiguration(new ContinentLockConfiguration());
            //builder.ApplyConfiguration(new ContinentUnlockConfiguration());
            builder.ApplyConfiguration(new DeathConfiguration());
            builder.ApplyConfiguration(new FacilityControlConfiguration());
            builder.ApplyConfiguration(new ExperienceGainConfiguration());
            //builder.ApplyConfiguration(new MetagameEventConfiguration());
            builder.ApplyConfiguration(new PlayerFacilityCaptureConfiguration());
            builder.ApplyConfiguration(new PlayerFacilityDefendConfiguration());
            builder.ApplyConfiguration(new PlayerLoginConfiguration());
            builder.ApplyConfiguration(new PlayerLogoutConfiguration());
            builder.ApplyConfiguration(new VehicleDestructionConfiguration());
            #endregion Event Data Configurations

            #region Census Configuration
            builder.ApplyConfiguration(new UpdaterSchedulerConfiguration());

            builder.ApplyConfiguration(new CharacterConfiguration());
            //builder.ApplyConfiguration(new ExperienceConfiguration());
            //builder.ApplyConfiguration(new FactionConfiguration());
            builder.ApplyConfiguration(new FacilityLinkConfiguration());
            builder.ApplyConfiguration(new FacilityTypeConfiguration());
            //builder.ApplyConfiguration(new ItemConfiguration());
            //builder.ApplyConfiguration(new ItemCategoryConfiguration());
            builder.ApplyConfiguration(new LoadoutConfiguration());
            //builder.ApplyConfiguration(new MetagameEventCategoryConfiguration());
            //builder.ApplyConfiguration(new MetagameEventStateConfiguration());
            builder.ApplyConfiguration(new MapRegionConfiguration());
            //builder.ApplyConfiguration(new ProfileConfiguration());
            //builder.ApplyConfiguration(new VehicleConfiguration());
            //builder.ApplyConfiguration(new WorldConfiguration());
            //builder.ApplyConfiguration(new ZoneConfiguration());
            #endregion Census Configuration
        }
    }
}
