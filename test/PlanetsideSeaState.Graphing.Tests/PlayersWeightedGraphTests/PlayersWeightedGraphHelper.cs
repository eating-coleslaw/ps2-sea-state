using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Graphing.Models;
using PlanetsideSeaState.Graphing.Models.Events;
using PlanetsideSeaState.Graphing.Models.Nodes;
using System;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Tests.PlayersWeightedGraphTests
{
    public static class PlayersWeightedGraphHelper
    {
        /*
         * Mock Facility Map:
         * 
         *         D          
         *         |          
         *         |          
         *         C----E----G
         *         | \  |     
         *   A-----B  \ |     
         *             `F     
         * 
         */

        public class FullGraphFixture //: IDisposable
        {
            public PlayersWeightedGraph Graph { get; private set; }
            private static readonly DateTime _seedTime = new(2021, 5, 1, 12, 0, 0);

            public FullGraphFixture()
            {
                Graph = new();
                
            }

            //public void Dispose()
            //{
            //    Graph.Dispose();
            //}

            public async Task Seed()
            {
                await Task.WhenAll(
                    Graph.AddNodeAsync(GetA(_seedTime)),
                    Graph.AddNodeAsync(GetB(_seedTime)),
                    Graph.AddNodeAsync(GetC(_seedTime)),
                    Graph.AddNodeAsync(GetD(_seedTime)),
                    Graph.AddNodeAsync(GetF(_seedTime)),
                    Graph.AddNodeAsync(GetE(_seedTime)),
                    Graph.AddNodeAsync(GetG(_seedTime))
                );

                await Task.WhenAll(
                Graph.AddOrUpdateRelationAsync(GetA(_seedTime), GetB(_seedTime), GetDeathAB(_seedTime)),  // A-B
                Graph.AddOrUpdateRelationAsync(GetB(_seedTime), GetC(_seedTime), GetDeathBC(_seedTime)),  // B-C
                Graph.AddOrUpdateRelationAsync(GetC(_seedTime), GetD(_seedTime), GetXpGainCD(_seedTime)), // C-D
                Graph.AddOrUpdateRelationAsync(GetC(_seedTime), GetE(_seedTime), GetXpGainCE(_seedTime)), // C-E
                Graph.AddOrUpdateRelationAsync(GetF(_seedTime), GetC(_seedTime), GetDeathCF(_seedTime)),  // C-F
                Graph.AddOrUpdateRelationAsync(GetE(_seedTime), GetF(_seedTime), GetDeathEF(_seedTime)),  // E-F
                Graph.AddOrUpdateRelationAsync(GetG(_seedTime), GetF(_seedTime), GetDeathEG(_seedTime))   // E-G
                );
            }
        }

        #region Player Nodes
        public static PlayerNode GetA(DateTime seedTime, uint zoneId = 1) => new(GetCharacterA(), seedTime, zoneId);
        public static PlayerNode GetB(DateTime seedTime, uint zoneId = 1) => new(GetCharacterB(), seedTime, zoneId);
        public static PlayerNode GetC(DateTime seedTime, uint zoneId = 1) => new(GetCharacterC(), seedTime, zoneId);
        public static PlayerNode GetD(DateTime seedTime, uint zoneId = 1) => new(GetCharacterD(), seedTime, zoneId);
        public static PlayerNode GetE(DateTime seedTime, uint zoneId = 1) => new(GetCharacterE(), seedTime, zoneId);
        public static PlayerNode GetF(DateTime seedTime, uint zoneId = 1) => new(GetCharacterF(), seedTime, zoneId);
        public static PlayerNode GetG(DateTime seedTime, uint zoneId = 1) => new(GetCharacterG(), seedTime, zoneId);
        #endregion Player Nodes

        #region Player Relation Events
        public static PlayerRelationEvent GetDeathAB(DateTime seedTime) => GetDeath(GetCharacterA(), GetCharacterB(), seedTime);
        public static PlayerRelationEvent GetDeathBC(DateTime seedTime) => GetDeath(GetCharacterB(), GetCharacterC(), seedTime);
        public static PlayerRelationEvent GetXpGainCD(DateTime seedTime) => GetExperienceGain(GetCharacterC(), GetCharacterD(), seedTime);
        public static PlayerRelationEvent GetXpGainCE(DateTime seedTime) => GetExperienceGain(GetCharacterA(), GetCharacterB(), seedTime);
        public static PlayerRelationEvent GetDeathCF(DateTime seedTime) => GetDeath(GetCharacterF(), GetCharacterC(), seedTime);
        public static PlayerRelationEvent GetDeathEF(DateTime seedTime) => GetDeath(GetCharacterE(), GetCharacterF(), seedTime);
        public static PlayerRelationEvent GetDeathEG(DateTime seedTime) => GetDeath(GetCharacterG(), GetCharacterF(), seedTime);

        public static PlayerRelationEvent GetDeath(Character attacker, Character victim, DateTime seedTime, short worldId = 17, uint zoneId = 1)
        {
            var death = new Death()
            {
                Timestamp = seedTime,
                AttackerCharacterId = attacker.Id,
                VictimCharacterId = victim.Id,
                AttackerCharacter = attacker,
                VictimCharacter = victim,
                ZoneId = zoneId,
                WorldId = worldId
            };

            return new PlayerRelationEvent(death);
        }

        public static PlayerRelationEvent GetExperienceGain(Character acting, Character recipient, DateTime seedTime, int experienceId = 2, short worldId = 17, uint zoneId = 1)
        {
            var xpGain = new ExperienceGain()
            {
                Timestamp = seedTime,
                CharacterId = acting.Id,
                OtherId = recipient.Id,
                ActingCharacter = acting,
                RecipientCharacter = recipient,
                ZoneId = zoneId,
                WorldId = worldId,
                ExperienceId = experienceId
            };

            return new PlayerRelationEvent(xpGain);
        }

        #endregion Player Relation Events

        #region Characters
        public static Character GetCharacterA()
        {
            return new()
            {
                Id = "1",
                FactionId = 1,
                Name = "VsA"
            };
        }

        public static Character GetCharacterB()
        {
            return new()
            {
                Id = "2",
                FactionId = 2,
                Name = "NcB"
            };
        }

        public static Character GetCharacterC()
        {
            return new()
            {
                Id = "3",
                FactionId = 3,
                Name = "TrC"
            };
        }

        public static Character GetCharacterD()
        {
            return new()
            {
                Id = "4",
                FactionId = 1,
                Name = "VsD"
            };
        }

        public static Character GetCharacterE()
        {
            return new()
            {
                Id = "5",
                FactionId = 2,
                Name = "NcE"
            };
        }

        public static Character GetCharacterF()
        {
            return new()
            {
                Id = "6",
                FactionId = 3,
                Name = "TrF"
            };
        }

        public static Character GetCharacterG()
        {
            return new()
            {
                Id = "7",
                FactionId = 1,
                Name = "VsG"
            };
        }
        #endregion Characters
    }
}
