using MyApp.Models.Messages;
using System;
using System.Collections.Generic;

namespace MyApp.Models
{
    public class GameState
    {
        public string lobby { get; set; }

        public DateTime created { get; set; }

        public GameStatus status { get; set; }

        public GameState(string lobbyName)
        {
            this.created = DateTime.UtcNow;
            this.status = GameStatus.PreGameLobby;
            this.lobby = lobbyName;
        }

        public List<Player> players { get; set; } = new List<Player>();
    }
    public enum GameStatus
    {
        PreGameLobby = 1,
        WarmUp = 2,
        InGame = 3,
        PostGame = 4
    }

}
