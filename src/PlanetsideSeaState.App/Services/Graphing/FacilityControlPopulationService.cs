using PlanetsideSeaState.App.Models;
using PlanetsideSeaState.Data.Models.QueryResults;
using PlanetsideSeaState.Data.Repositories;
using PlanetsideSeaState.Graphing.Models;
using PlanetsideSeaState.Graphing.Models.Nodes;
using PlanetsideSeaState.Shared.Constants;
using PlanetsideSeaState.Shared.Planetside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Graphing
{
    public class FacilityControlPopulationService : IFacilityControlPopulationService
    {
        private readonly IEventRepository _eventRepository;

        private PlayersWeightedGraph Graph { get; set; }

        public FacilityControlPopulationService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<FacilityControlPopulations> GetFacilityControlPopulationsAsync(Guid facilityControlId)
        {
            // 1. get Facility Control info
            // 2. get Attributed Players info
            var facilityControl = await _eventRepository.GetFacilityControlWithAttributedPlayers(facilityControlId);

            if (facilityControl == null)
            {
                return null;
            }

            var populations = new FacilityControlPopulations(facilityControl);

            var endTime = facilityControl.Timestamp;
            var startTime = endTime - TimeSpan.FromMinutes(5);
            var worldId = facilityControl.WorldId;
            var zoneId = facilityControl.ZoneId;

            // 3. get Player Connection Events
            var playerEvents = await _eventRepository.GetPlayerConnectionEventsAsync(startTime, endTime, worldId, zoneId);

            // 4. loop through Player Connection Events to populate the Graph
            foreach (var ev in playerEvents)
            {
                GetOrCreatePlayerNodes(ev, out PlayerNode actingNode, out PlayerNode recipientNode);
                
                // Update the teamId field, if possible from this player connection event
                if (ev.ExperienceId.HasValue && Experience.IsFriendlyIdentifier(ev.ExperienceId.Value))
                {
                    // If either character is not NSO, update their team IDs.
                    // Only update NSO team IDs from non-NSO characters, to prevent propagating bad team ID values
                    if (actingNode.FactionId == Faction.NSO && recipientNode.FactionId != Faction.NSO && actingNode.TeamId.Value != recipientNode.FactionId)
                    {
                        actingNode.TeamId = recipientNode.FactionId;
                    }

                    if (recipientNode.FactionId == Faction.NSO && actingNode.FactionId != Faction.NSO && recipientNode.TeamId.Value != actingNode.FactionId)
                    {
                        recipientNode.TeamId = actingNode.FactionId;
                    }
                }

                Graph.AddOrUpdateRelation(actingNode, recipientNode, ev);
            }

            // 5. search through the Graph to get the population counts


            return populations;
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

            recipientNode = Graph.GetPlayerNode(ev.ActingCharacterId);
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
