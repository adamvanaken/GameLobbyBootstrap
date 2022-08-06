using MyApp.Models;
using System;

namespace MyApp.Helpers
{
    public static class GameStateManager
    {
        public static string GetPlayerName(this GameState game, Guid playerId)
        {
            for (int i = 0; i < game.players.Count; i++)
            {
                if (game.players[i].uuid == playerId)
                {
                    return game.players[i].name;
                }
            }

            return "System";
        }
    }
}
