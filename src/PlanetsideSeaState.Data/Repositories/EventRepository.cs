using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Models.QueryResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public EventRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        // Credit to Lampjaw
        public async Task AddAsync<T>(T entity) where T : class
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            try
            {
                dbContext.Add(entity);
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as PostgresException)?.SqlState == "23505")
            {
                // Ignore unique constraint errors (https://www.postgresql.org/docs/current/static/errcodes-appendix.html)
            }
        }

        public async Task<IEnumerable<Death>> GetDeathsForWorldInTimeRange(short worldId, DateTime start, DateTime end)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var deaths = new List<Death>();

            try
            {
                return await dbContext.Deaths.Where(d => d.WorldId == worldId
                                                      && d.Timestamp >= start
                                                      && d.Timestamp <= end)
                                             .ToListAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<DeathWithExperience>> GetDeathWithExperienceForWorldInTimeRange(short worldId, DateTime start, DateTime end)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();


            // Exp Gains: 1 - Player Kill, 277 - Spawn Kill, 279 - High Priority Kill
            // TODO: confirm that Spawn Kill gains are non-redundant with 1/279 kills
            var killExpGainsQuery = from expGain in dbContext.ExperienceGains
                                    where (expGain.ExperienceId == 1
                                            || expGain.ExperienceId == 277
                                            || expGain.ExperienceId == 279)
                                          && expGain.WorldId == worldId
                                          && expGain.Timestamp >= start
                                          && expGain.Timestamp <= end
                                    select expGain;

            var query = from death in dbContext.Deaths
                        join expGain in killExpGainsQuery on new
                            {
                                death.Timestamp,
                                death.AttackerCharacterId,
                                death.VictimCharacterId
                                //ExperienceId = 1
                            } equals
                                new
                                {
                                    expGain.Timestamp,
                                    AttackerCharacterId = expGain.CharacterId,
                                    VictimCharacterId = expGain.OtherId
                                    //expGain.ExperienceId
                                } into withExpGains
                        from withExpGain in withExpGains.DefaultIfEmpty()

                        where death.WorldId == worldId
                            && death.Timestamp >= start
                            && death.Timestamp <= end
                            //&& (withExpGain.ExperienceId == 1 || withExpGain.ExperienceId == 0)
                            //&& withExpGain.WorldId == worldId
                            //&& withExpGain.Timestamp >= start
                            //&& withExpGain.Timestamp <= end

                        select new DeathWithExperience
                        {
                            Timestamp = death.Timestamp,
                            AttackerCharacterId = death.AttackerCharacterId,
                            AttackerFactionId = death.AttackerFactionId,
                            AttackerLoadoutId = death.AttackerLoadoutId,
                            AttackerVehicleId = death.AttackerVehicleId,
                            VictimCharacterId = death.VictimCharacterId,
                            VictimFactionId = death.VictimFactionId,
                            VictimLoadoutId = death.VictimLoadoutId,
                            IsHeadshot = death.IsHeadshot,
                            WeaponId = death.WeaponId,
                            WorldId = death.WorldId,
                            ZoneId = death.ZoneId,
                            ExperienceId = withExpGain == null ? null : withExpGain.ExperienceId,
                            ExperienceAmount = withExpGain == null ? null : withExpGain.Amount
                        };

            var resultSet = await query
                                    .AsNoTracking()
                                    .ToListAsync();

            var sameFactionCount = resultSet.Count(r => r.AttackerFactionId == r.VictimFactionId);

            var sameFactionNoExpCount = resultSet.Count(r => r.AttackerFactionId == r.VictimFactionId && (r.ExperienceAmount == 0 || r.ExperienceAmount == null));

            var sameFactionNoExpCountNonNso = resultSet.Count(r => r.AttackerFactionId == r.VictimFactionId && r.AttackerFactionId != 4 && (r.ExperienceAmount == 0 || r.ExperienceAmount == null));
            
            var sameFactionExpCountNso = resultSet.Count(r => r.AttackerFactionId == r.VictimFactionId && r.AttackerFactionId == 4 && (r.ExperienceAmount != 0 && r.ExperienceAmount != null));
            
            var sameFactionNoExpCountNso = resultSet.Count(r => r.AttackerFactionId == r.VictimFactionId && r.AttackerFactionId == 4 && (r.ExperienceAmount == 0 || r.ExperienceAmount == null));

            var nullExpAmount = resultSet.Count(r => r.ExperienceAmount == null);

            var zeroExpAmount = resultSet.Count(r => r.ExperienceAmount == 0);

            Console.WriteLine($"Same Faction Count: {sameFactionCount} | {sameFactionNoExpCount} | {sameFactionNoExpCountNonNso} | {sameFactionExpCountNso} | {sameFactionNoExpCountNso} | {nullExpAmount} | {zeroExpAmount} ");

            return resultSet;
            
            //return await query
            //                .AsNoTracking()
            //                .ToListAsync();
        }
    }
}
