using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Graphing.Exceptions;
using PlanetsideSeaState.Graphing.Models.Events;
using PlanetsideSeaState.Graphing.Models.Nodes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Models
{
    public class FacilityPopGraph
    {
        private FacilityGraph FacilityGraph { get; set; } = new();
        private PlayersGraph PlayersGraph { get; set; } = new();
        private PlayerFacilityGraph PlayerFacilityGraph { get; set; } = new();

        public Dictionary<int, FacilityNode> FacilityIdMap { get; } = new();
        public ConcurrentDictionary<string, PlayerNode> PlayerIdMap { get; } = new();


        public int FacilityCount => FacilityGraph.FacilityCount;
        public int FacilityLinkCount => FacilityGraph.EdgeCount;

        public int PlayerCount => PlayersGraph.PlayerCount;
        public int PlayerRelationCount => PlayersGraph.EdgeCount / 2;
        public int FacilityRelationCount => PlayerFacilityGraph.EdgeCount;


        public async Task AddFacility(FacilityNode facilityNode)
        {
            if (FacilityIdMap.ContainsKey(facilityNode.Id))
            {
                throw new DuplicateGraphNodeException();
            }

            FacilityIdMap.Add(facilityNode.Id, facilityNode);

            FacilityGraph.AddNode(facilityNode);

            await PlayerFacilityGraph.AddNode(facilityNode);
        }

        public FacilityNode GetFacilityNode(int facilityId)
        {
            return FacilityIdMap[facilityId];
        }

        public void AddFacilityConnection(int lhsFacilityId, int rhsFacilityId)
        {
            FacilityGraph.AddConnection(lhsFacilityId, rhsFacilityId);
        }

        public void MarkFacilityMapComplete()
        {
            FacilityGraph.MarkMapComplete();
        }

        public void MarkFacilityMapIncomplete()
        {
            FacilityGraph.MarkMapIncomplete();
        }

        public async Task UpdateFacilityOwner(FacilityOwnerUpdate update)
        {
            await FacilityGraph.UpdateFacilityOwner(update);
        }


        public async Task UpdateFacilityRelation(FacilityRelationEvent relationEvent)
        {
            if (relationEvent.Character is null)
            {
                // throw new exception
                return;
            }

            var playerNode = await AddOrUpdatePlayerInternal(relationEvent.Character, relationEvent.ZoneId, relationEvent.Timestamp);

            await AddOrUpdateFacilityRelationInternal(playerNode, relationEvent);
        }

        private async Task AddOrUpdateFacilityRelationInternal(PlayerNode playerNode, FacilityRelationEvent relationEvent)
        {
            if (!FacilityIdMap.ContainsKey(relationEvent.FacilityId))
            {
                throw new KeyNotFoundException($"Graph does not contain a facility with ID {relationEvent.FacilityId}");
            }

            var facilityNode = GetFacilityNode(relationEvent.FacilityId);

            await PlayerFacilityGraph.AddOrUpdateRelation(playerNode, facilityNode, relationEvent);
        }

        public async Task UpdatePlayerRelation(PlayerRelationEvent relationEvent)
        {
            if (!relationEvent.ZoneId.HasValue)
            {
                return;
            }

            var hasActingPlayer = relationEvent.ActingCharacter != null;
            var hasRecipientPlayer = relationEvent.RecipientCharacter != null;

            PlayerNode actingNode = null;
            PlayerNode recipientNode = null;

            if (hasActingPlayer)
            {
                actingNode = await AddOrUpdatePlayerInternal(relationEvent.ActingCharacter, relationEvent.ZoneId.Value, relationEvent.Timestamp);
            }

            if (hasRecipientPlayer)
            {
                recipientNode = await AddOrUpdatePlayerInternal(relationEvent.RecipientCharacter, relationEvent.ZoneId.Value, relationEvent.Timestamp);
            }

            if (actingNode != null && recipientNode != null)
            {
                await PlayersGraph.AddOrUpdateRelation(actingNode, recipientNode, relationEvent);
            }
        }


        private async Task<PlayerNode> AddOrUpdatePlayerInternal(Character character, int zoneId, DateTime timestamp)
        {
            if (!PlayerIdMap.TryGetValue(character.Id, out var playerNode))
            {
                playerNode = new PlayerNode(character, timestamp, zoneId);

                PlayerIdMap.TryAdd(character.Id, playerNode);

                await PlayersGraph.AddNode(playerNode);

                await PlayerFacilityGraph.AddNodeAsync(playerNode);
            }
            else
            {
                var zoneUpdate = new PlayerZoneUpdate(playerNode.Id, zoneId, timestamp);

                await PlayersGraph.UpdatePlayerZone(playerNode, zoneUpdate);
            }

            return playerNode;
        }
    }
}
