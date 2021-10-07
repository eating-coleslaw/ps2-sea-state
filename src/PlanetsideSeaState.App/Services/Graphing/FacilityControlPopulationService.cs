using Microsoft.Extensions.Logging;
using PlanetsideSeaState.App.Models;
using PlanetsideSeaState.Data.Models.QueryResults;
using PlanetsideSeaState.Data.Repositories;
using PlanetsideSeaState.Graphing.Models;
using PlanetsideSeaState.Graphing.Models.Nodes;
using PlanetsideSeaState.Shared.Constants;
using PlanetsideSeaState.Shared.Planetside;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Graphing
{
    public class FacilityControlPopulationService : IFacilityControlPopulationService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<FacilityControlPopulationService> _logger;

        private PlayersWeightedGraph Graph { get; set; } //= new();

        // This is intended to act as a concurrent HashSet. The value type is byte because it's a small value type
        private ConcurrentDictionary<PlayerNode, int> VisitedPlayers { get; set; } //= new();
        private ConcurrentDictionary<PlayerNode, int> VisitedPlayersMinDistances { get; set; }
        //private ConcurrentDictionary<PlayerNode, byte> AttributedPlayerNodes { get; set; } //= new();
        private List<PlayerNode> AttributedPlayerNodes { get; set; } //= new();

        private readonly int MaxSearchDepth = 10;
        private readonly TimeSpan MaxEventTimeSpan = TimeSpan.FromMinutes(1);

        private int Total { get; set; }
        private int Vs { get; set; }
        private int Nc { get; set; }
        private int Tr { get; set; }
        private int Unknown { get; set; }
        private int Nso { get; set; }
        private int Nso_Vs { get; set; }
        private int Nso_Nc { get; set; }
        private int Nso_Tr { get; set; }
        private int Nso_Unknown { get; set; }

        public FacilityControlPopulationService(IEventRepository eventRepository, ILogger<FacilityControlPopulationService> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;

            Graph = new();
            VisitedPlayers = new();
            VisitedPlayersMinDistances = new();
            AttributedPlayerNodes = new();
        }

        public async Task<FacilityControlPopulations> GetFacilityControlPopulationsAsync(Guid facilityControlId)
        {
            Stopwatch stopWatch = new();
            stopWatch.Start();

            // 1. get Facility Control info
            // 2. get Attributed Players info
            var facilityControl = await _eventRepository.GetFacilityControlWithAttributedPlayers(facilityControlId);

            if (facilityControl == null)
            {
                return null;
            }

            var populations = new FacilityControlPopulations(facilityControl);

            // If there are no attributed players, then all population counts will be zero
            if (facilityControl.PlayerControls.Count == 0)
            {
                stopWatch.Stop();
                populations.ElapsedMilliseconds = stopWatch.ElapsedMilliseconds;

                return populations;
            }

            var attributedPlayers = facilityControl.PlayerControls.OrderBy(e => e.Timestamp).ToArray();

            var endTime = facilityControl.Timestamp;
            //var startTime = endTime - TimeSpan.FromMinutes(5);
            var startTime = endTime - TimeSpan.FromMinutes(3);
            var worldId = facilityControl.WorldId;
            var zoneId = facilityControl.ZoneId;

            // 3. get Player Connection Events
            var playerEvents = await _eventRepository.GetPlayerConnectionEventsAsync(startTime, endTime, worldId, zoneId);

            populations.SearchBaseStartTime = startTime;
            populations.SearchBaseEndTime = endTime;

            Dictionary<PayloadEventType, int> eventTypeCounts = new();

            // 4. loop through Player Connection Events to populate the Graph
            foreach (var ev in playerEvents)
            {
                if (string.IsNullOrWhiteSpace(ev.ActingCharacterId) || string.IsNullOrWhiteSpace(ev.RecipientCharacterId))
                {
                    continue;
                }

                populations.SearchBasePlayerEvents++;
                if (!eventTypeCounts.ContainsKey(ev.EventType))
                {
                    eventTypeCounts.Add(ev.EventType, 1);
                }
                else
                {
                    eventTypeCounts[ev.EventType] += 1;
                }

                GetOrCreatePlayerNodes(ev, out PlayerNode actingNode, out PlayerNode recipientNode);
                
                // Update the teamId field, if possible from this player connection event
                if (ev.ExperienceId.HasValue && Experience.IsFriendlyIdentifier(ev.ExperienceId.Value))
                {
                    // If either character is not NSO, update their team IDs.
                    // Only update NSO team IDs from non-NSO characters, to prevent propagating bad team ID values
                    if (actingNode.FactionId == Faction.NSO
                        && recipientNode.FactionId != Faction.NSO
                        && (!actingNode.TeamId.HasValue || actingNode.TeamId.Value != recipientNode.FactionId))
                    {
                        actingNode.TeamId = recipientNode.FactionId;
                    }

                    if (recipientNode.FactionId == Faction.NSO
                        && actingNode.FactionId != Faction.NSO
                        && (!recipientNode.TeamId.HasValue || recipientNode.TeamId.Value != actingNode.FactionId))
                    {
                        recipientNode.TeamId = actingNode.FactionId;
                    }
                }

                Graph.AddOrUpdateRelation(actingNode, recipientNode, ev);
            }

            // 4.5 get all PlayerNodes for attributed players
            foreach (var player in attributedPlayers)
            {
                var playerNode = Graph.GetPlayerNode(player.CharacterId);
                
                // The player contributed to the facility control in a manner we don't track
                if (playerNode == null)
                {
                    playerNode = new PlayerNode(player.CharacterId, string.Empty, facilityControl.NewFactionId, facilityControl.Timestamp, facilityControl.ZoneId, facilityControl.NewFactionId);
                }

                playerNode.TeamId = facilityControl.NewFactionId;

                AttributedPlayerNodes.Add(playerNode);
            }

            // 5. search through the Graph to get the population counts
            foreach (var playerNode in AttributedPlayerNodes)
            {
                var localVisitedPlayers = new ConcurrentDictionary<PlayerNode, byte>();

                SearchGraph(playerNode, 0, facilityControl.Timestamp, localVisitedPlayers);
            }

            populations.TotalPlayers = Total;
            populations.FactionPlayers["VS"] = Vs;
            populations.FactionPlayers["NC"] = Nc;
            populations.FactionPlayers["TR"] = Tr;
            populations.FactionPlayers["Unknown"] = Unknown;

            populations.NsoPlayers = Nso;
            populations.NsoFactionPlayers["VS"] = Nso_Vs;
            populations.NsoFactionPlayers["NC"] = Nso_Nc;
            populations.NsoFactionPlayers["TR"] = Nso_Tr;
            populations.NsoFactionPlayers["Unknown"] = Nso_Unknown;

            populations.SearchBasePlayerEventTypes = new();

            foreach (var typeCount in eventTypeCounts)
            {
                var name = Enum.GetName<PayloadEventType>(typeCount.Key);
                var count = typeCount.Value;

                populations.SearchBasePlayerEventTypes[name] = count;
            }

            stopWatch.Stop();
            populations.ElapsedMilliseconds = stopWatch.ElapsedMilliseconds;

            Console.WriteLine($"____Visited {VisitedPlayers.Count}____");
            Console.WriteLine($"ID \t\t\t Visited   Dist. Faction   Is Attr.");
            foreach (var entry in VisitedPlayers.Keys.OrderBy(e => e.TeamId).ThenBy(e => e.LastSeen).ThenBy(e => e.Id))
            {
                Console.WriteLine($"{entry.Id}: \t {VisitedPlayers[entry]} \t   {VisitedPlayersMinDistances[entry]} \t [{Faction.GetAbbreviation(entry.TeamId)}] \t   {(AttributedPlayerNodes.Contains(entry) ? "(attr)" : string.Empty)}");
            }

            return populations;
        }

        private void SearchGraph(PlayerNode parent, int depth, DateTime baseTimestamp, ConcurrentDictionary<PlayerNode, byte> localVisitedPlayers)
        {
            if (!localVisitedPlayers.TryAdd(parent, 0))
            {
                return;
            }

            if (VisitedPlayers.TryAdd(parent, 1))
            {
                VisitedPlayersMinDistances.TryAdd(parent, depth);

                Total++;

                switch (parent.TeamId)
                {
                    case Faction.VS:
                        Vs++;
                        if (parent.FactionId == Faction.NSO)
                        {
                            Nso_Vs++;
                            Nso++;
                        }
                        break;

                    case Faction.NC:
                        Nc++;
                        if (parent.FactionId == Faction.NSO)
                        {
                            Nso_Nc++;
                            Nso++;
                        }
                        break;

                    case Faction.TR:
                        Tr++;
                        if (parent.FactionId == Faction.NSO)
                        {
                            Nso_Tr++;
                            Nso++;
                        }
                        break;

                    default:
                        Unknown++;
                        if (parent.FactionId == Faction.NSO)
                        {
                            Nso_Unknown++;
                            Nso++;
                        }
                        break;
                }
            }
            else
            {
                VisitedPlayers[parent] += 1;
                VisitedPlayersMinDistances[parent] = Math.Min(depth, VisitedPlayersMinDistances[parent]);
            }


            if (depth >= MaxSearchDepth)
            {
                return;
            }

            var connections = parent.Connections;
                
            foreach (var connection in connections)
            {
                var timeDiff = baseTimestamp - connection.LastUpdate;
                if (timeDiff > MaxEventTimeSpan)
                {
                    continue;
                }

                var childNode = connection.Child;

                // Skip continuing search from attributed players' nodes as they will already be
                // the starting point for their own graph search.
                if (AttributedPlayerNodes.Contains(childNode))
                {
                    continue;
                }

                SearchGraph(childNode, depth + 1, baseTimestamp, localVisitedPlayers);
            }
        }

        private void GetOrCreatePlayerNodes(PlayerConnectionEvent ev, out PlayerNode actingNode, out PlayerNode recipientNode)
        {
            actingNode = Graph.GetPlayerNode(ev.ActingCharacterId);
            if (actingNode == null)
            {
                var teamId = ev.ActingFactionId == 4 ? (short?)null : ev.ActingFactionId;
                actingNode = new PlayerNode(ev.ActingCharacterId, string.Empty, ev.ActingFactionId, ev.Timestamp, ev.ZoneId, teamId);
                Graph.AddNode(actingNode);
            }

            recipientNode = Graph.GetPlayerNode(ev.RecipientCharacterId);
            if (recipientNode == null)
            {
                var teamId = ev.RecipientFactionId == 4 ? (short?)null : ev.RecipientFactionId;
                recipientNode = new PlayerNode(ev.RecipientCharacterId, string.Empty, ev.RecipientFactionId, ev.Timestamp, ev.ZoneId, teamId);
                Graph.AddNode(recipientNode);
            }
        }

        public Task<FacilityControlPopulations> GetFacilityControlPopulationsAsync(DateTime timestamp, int facilityId, short worldId)
        {
            throw new NotImplementedException();
        }
    }
}
