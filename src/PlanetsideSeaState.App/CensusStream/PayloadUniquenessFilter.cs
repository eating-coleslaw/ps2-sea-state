﻿using PlanetsideSeaState.Shared;
using PlanetsideSeaState.App.CensusStream.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream
{
    public class PayloadUniquenessFilter<T> where T : PayloadBase, IEquatablePayload<T>
    {
        private ConcurrentQueue<T> PayloadQueue { get; set; } = new ConcurrentQueue<T>();
        private readonly KeyedSemaphoreSlim _payloadLock = new KeyedSemaphoreSlim();

        private int MaxQueueItems { get; set; } = 15;

        public PayloadUniquenessFilter(int maxQueueItems = 15)
        {
            MaxQueueItems = maxQueueItems;
        }

        public async Task<bool> TryFilterNewPayload(T payload, Func<T, string> keyExpression)
        {
            var payloadKey = $"{typeof(T).Name}:{keyExpression(payload)}";

            using (await _payloadLock.WaitAsync(payloadKey))
            {
                if (PayloadQueue.Contains(payload))
                {
                    return false;
                }
                else if (PayloadQueue.Count < MaxQueueItems)
                {
                    PayloadQueue.Enqueue(payload);
                    return true;
                }
                else
                {
                    PayloadQueue.TryDequeue(out _);
                    PayloadQueue.Enqueue(payload);
                    return true;
                }
            }
        }
    }
}
