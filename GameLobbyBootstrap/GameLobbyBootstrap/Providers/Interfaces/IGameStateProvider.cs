using MyApp.Helpers;
using System;

namespace GameLobbyBootstrap.Providers.Interfaces
{
    public interface IGameStateProvider
    {
        string GetGameKey(string roomKey);
        ConcurrentGameState? GetGameState(string key);
        Guid[]? GetPlayerIds(string groupName);
        int JoinRoom(string playerName, string lobbyName, Guid playerId);
        void UpdateGameState(string key, ConcurrentGameState game);
        int GetGameCount();
    }
}