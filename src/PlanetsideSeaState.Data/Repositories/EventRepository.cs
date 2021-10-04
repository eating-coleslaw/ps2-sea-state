using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Models.QueryResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PlanetsideSeaState.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly IDbHelper _dbHelper;
        private readonly ILogger<EventRepository> _logger;
        private readonly IDataReader<FacilityControlInfo> _controlDataReader;
        private readonly IDataReader<PlayerConnectionEvent> _playerConnectionDataReader;

        public EventRepository(
            IDbContextHelper dbContextHelper,
            IDbHelper dbHelper,
            IDataReader<FacilityControlInfo> controlDataReader,
            IDataReader<PlayerConnectionEvent> playerConnectionDataReader,
            ILogger<EventRepository> logger
        )
        {
            _dbContextHelper = dbContextHelper;
            _dbHelper = dbHelper;
            _logger = logger;
            _controlDataReader = controlDataReader ?? throw new ArgumentNullException(nameof(controlDataReader));
            _playerConnectionDataReader = playerConnectionDataReader ?? throw new ArgumentNullException(nameof(playerConnectionDataReader));
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

        public async Task<FacilityControl> GetFacilityControl(int facilityId, DateTime timestamp, short worldId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            try
            {
                return await dbContext.FacilityControls
                                        .Where(e => e.FacilityId == facilityId
                                                 && e.Timestamp == timestamp
                                                 && e.WorldId == worldId)
                                        .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetFacilityControl: {ex}");
                return null;
            }
        }

        public async Task<FacilityControl> GetFacilityControlAsync(Guid id)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            try
            {
                return await dbContext.FacilityControls.FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetFacilityControlWithAttributedPlayers(Guid): {ex}");
                return null;
            }
        }

        public async Task<IEnumerable<PlayerFacilityControl>> GetFacilityControlAttributedPlayers(Guid id)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            try
            {
                var facilityControl = await dbContext.FacilityControls.FirstOrDefaultAsync(e => e.Id == id);
                
                if (facilityControl == null)
                {
                    return null;
                }

                return await dbContext.PlayerFacilityControls
                                        .Where(e => e.FacilityId == facilityControl.FacilityId
                                                 && e.Timestamp == facilityControl.Timestamp
                                                 && e.WorldId == facilityControl.WorldId)
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetFacilityControlWithAttributedPlayers(Guid): {ex}");
                return null;
            }
        }

        public async Task<IEnumerable<PlayerFacilityControl>> GetFacilityControlAttributedPlayers(int facilityId, DateTime timestamp, short worldId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            try
            {
                return await dbContext.PlayerFacilityControls
                                        .Where(e => e.FacilityId == facilityId
                                                 && e.Timestamp == timestamp
                                                 && e.WorldId == worldId)
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetFacilityControlAttributedPlayers(int, DateTime, short): {ex}");
                return null;
            }
        }

        public async Task<FacilityControl> GetFacilityControlWithAttributedPlayers(Guid id)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            try
            {
                return await dbContext.FacilityControls
                                        .Where(e => e.Id == id)
                                        .Include("PlayerControls")
                                        .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetFacilityControlWithAttributedPlayers(Guid): {ex}");
                return null;
            }
        }

        public async Task<FacilityControl> GetFacilityControlWithAttributedPlayers(int facilityId, DateTime timestamp, short worldId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            try
            {
                return await dbContext.FacilityControls
                                        .Where(e => e.FacilityId == facilityId
                                                 && e.Timestamp == timestamp
                                                 && e.WorldId == worldId)
                                        .Include("PlayerControls")
                                        .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetFacilityControlWithAttributedPlayers(int, DateTime, short): {ex}");
                return null;
            }
        }

        public async Task<IEnumerable<FacilityControlInfo>> GetRecentFacilityControlsAsync(short? worldId, int? facilityId, short? rowLimit)
        {
            using NpgsqlConnection connection = _dbHelper.CreateConnection();
            
            using NpgsqlCommand cmd = await _dbHelper.CreateTextCommand(connection, @"SELECT * FROM GetRecentFacilityControls(@worldId, @facilityId, @rowLimit);");

            cmd.AddParameter("worldId", worldId);
            cmd.AddParameter("facilityId", facilityId);
            cmd.AddParameter("rowLimit", rowLimit);

            IEnumerable<FacilityControlInfo> events = await _controlDataReader.ReadList(cmd);
            await connection.CloseAsync();

            return events;
        }
        
        public async Task<IEnumerable<PlayerConnectionEvent>> GetPlayerConnectionEventsAsync(DateTime start, DateTime end, short worldId, uint? zoneId)
        {
            using NpgsqlConnection connection = _dbHelper.CreateConnection();
            
            using NpgsqlCommand cmd = await _dbHelper.CreateTextCommand(connection, @"SELECT * FROM GetPlayerConnectionEvents(@start, @end, @worldId, @zoneId);");

            cmd.AddParameter("start", start);
            cmd.AddParameter("end", end);
            cmd.AddParameter("worldId", worldId);
            cmd.AddParameter("zoneId", zoneId);

            IEnumerable<PlayerConnectionEvent> events = await _playerConnectionDataReader.ReadList(cmd);
            await connection.CloseAsync();

            return events;
        }
    }
}
