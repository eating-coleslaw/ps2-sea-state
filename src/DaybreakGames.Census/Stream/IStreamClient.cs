// Credit to Lampjaw

using System;
using System.Threading.Tasks;
using Websocket.Client;

namespace DaybreakGames.Census.Stream
{
    public interface IStreamClient : IDisposable
    {
        StreamClient OnDisconnect(Func<DisconnectionInfo, Task> onDisconnect);
        StreamClient OnMessage(Func<string, Task> onMessage);
        Task ConnectAsync();
        Task DisconnectAsync();
        Task ReconnectAsync();
        StreamClient OnConnect(Func<ReconnectionType, Task> onConnect);
        void Subscribe(CensusStreamSubscription subscription);
    }
}