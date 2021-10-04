using Microsoft.Extensions.Logging;
using PlanetsideSeaState.Graphing.Models;
using PlanetsideSeaState.Graphing.Models.Events;
using PlanetsideSeaState.Graphing.Models.Nodes;
using PlanetsideSeaState.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Services.PlayersGraph
{
    public class PlayersGraphService : StatefulService, IDisposable
    {
        //private readonly ICharacterService _characterService;
        //private readonly ILogger<FacilityPopGraphService> _logger;


        private PlayersWeightedGraph PlayersGraph { get; set; } = new();
        private ConcurrentDictionary<string, PlayerNode> PlayerNodesMap { get; set; } = new();

        private bool IsCatchingUp { get; set; }
        private bool PauseUpdates { get; set; } = true;
        private ConcurrentQueue<PlayerRelationEvent> PlayerRelationsQueue { get; set; } = new();

        public int ServiceKey { get; private set; }
        private int WorldId => ServiceKey;
        public override string ServiceName => $"FacilityPopGraphService_{ServiceKey}";

        private bool _initializedValue;

        private readonly AutoResetEvent _autoEvent = new(true);


        public void SetServiceKey(int serviceKey)
        {
            if (!_initializedValue)
            {
                ServiceKey = serviceKey;
                _initializedValue = true;
            }
        }

        #region Stream Update Handling
        public async Task ReceivePlayerRelationEvent(PlayerRelationEvent relationEvent)
        {
            if (relationEvent.ZoneId == null)
            {
                return;
            }
            
            if (PauseUpdates)
            {
                PlayerRelationsQueue.Enqueue(relationEvent);
                return;
            }

            await HandlePlayerRelationEventInternal(relationEvent);
        }
        
        private async Task HandlePlayerRelationEventInternal(PlayerRelationEvent relationEvent)
        {
            var addNodesTasks = new List<Task>();
            
            if (!PlayerNodesMap.TryGetValue(relationEvent.ActingCharacter.Id, out PlayerNode actingNode))
            {
                actingNode = new PlayerNode(relationEvent.ActingCharacter, relationEvent.Timestamp, relationEvent.ZoneId.Value);
                addNodesTasks.Add(PlayersGraph.AddNodeAsync(actingNode));
            }

            if (!PlayerNodesMap.TryGetValue(relationEvent.RecipientCharacter.Id, out PlayerNode recipientNode))
            {
                recipientNode = new PlayerNode(relationEvent.RecipientCharacter, relationEvent.Timestamp, relationEvent.ZoneId.Value);
                addNodesTasks.Add(PlayersGraph.AddNodeAsync(recipientNode));
            }

            if (addNodesTasks.Any())
            {
                await Task.WhenAll(addNodesTasks);
            }

            await PlayersGraph.AddOrUpdateRelationAsync(actingNode, recipientNode, relationEvent);
        }

        public async Task ProcessUpdatesQueue()
        {
            _autoEvent.WaitOne();

            try
            {
                PauseUpdates = true;

                while(PlayerRelationsQueue.TryDequeue(out PlayerRelationEvent relationEvent))
                {
                    await HandlePlayerRelationEventInternal(relationEvent);
                }

            }
            finally
            {
                PauseUpdates = false;
                _autoEvent.WaitOne();
            }

        }
        #endregion Stream Update Handling

        #region StatefulService Implementation
        public override async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            PauseUpdates = true;

            if (!_initializedValue)
            {
                // TODO: throw new exception
                return;
            }

            PauseUpdates = false;

            // start processing queues
            await ProcessUpdatesQueue();
        }

        public override Task StopInternalAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        #endregion StatefulService Implementation

        #region IDisposable Implementation
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PlayersGraphService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}
