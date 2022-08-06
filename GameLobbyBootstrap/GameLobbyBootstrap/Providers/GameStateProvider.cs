using GameLobbyBootstrap.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using MyApp.Helpers;
using MyApp.Models;
using MyApp.Models.Messages;
using System;
using System.Linq;
using System.Threading;

namespace MyApp.Providers
{
    public class GameStateProvider : IGameStateProvider
    {
        private object _lockObject;
        private MemoryCache _gameCache;
        private MemoryCache _lockCache;

        public GameStateProvider()
        {
            // TODO: move these to a database with appropriate concurrency management
            // In-memory expiring cache for gamestate objects
            _gameCache = new MemoryCache(new MemoryCacheOptions()
            {
                // A unitless limit; each addition to this cache can specify size. It could be, for example, a constant size per game
                //  or perhaps a function of player count, depending on how desired memory constraints
                SizeLimit = 200,
                // How often to check for and remove expired entires
                ExpirationScanFrequency = TimeSpan.FromMinutes(5)
            });

            // In-memory expiring cache for locks corresponding to each gamestate
            _lockCache = new MemoryCache(new MemoryCacheOptions()
            {
                SizeLimit = 200,
                ExpirationScanFrequency = TimeSpan.FromMinutes(5)
            });

            // A single lock object for interacting with the above caches.
            // Note that this will be a potential bottleneck for more concurrency, 
            //  but in general these interactions should be refactored to not be handled
            //  in-memory when at scale.
            _lockObject = new object();
        }

        public int JoinRoom(string playerName, string lobbyName, Guid playerId)
        {
            var roomKey = GetGameKey(lobbyName);

            lock (_lockObject)
            {
                if (_gameCache.TryGetValue(roomKey, out GameState waitingRoomGame))
                {
                    // TODO: Configurable player limit
                    if (waitingRoomGame.players.Count == 30)
                    {
                        waitingRoomGame.players.Add(new Player(playerName, waitingRoomGame.players.Count, playerId));
                        _gameCache.Remove(roomKey);

                        // TODO: reconsider locking logic here, currently it double-locks?
                        return StartGame(waitingRoomGame);
                    }
                    else
                    {
                        waitingRoomGame.players.Add(new Player(playerName, waitingRoomGame.players.Count, playerId));

                        _gameCache.Set(roomKey, waitingRoomGame, GetDefaultOptions());
                        return 0;
                    }
                }
                else
                {
                    // Not found, so let's init and add the first player
                    waitingRoomGame = new GameState(lobbyName);
                    waitingRoomGame.players.Add(new Player(playerName, 0, playerId));

                    _gameCache.Set(roomKey, waitingRoomGame, GetDefaultOptions());
                    return 0;
                }
            }
        }

        private int StartGame(GameState gameState)
        {
            var rand = new Random();
            var roomKey = rand.Next();

            lock (_lockObject)
            {
                // While this random room key matches an existing room, attempt to search for a different one
                while (_gameCache.TryGetValue(GetGameKey(gameState.lobby), out _))
                {
                    roomKey = rand.Next();
                }

                // Got a unique name so add it and leave the lock. We can now more safely access this game
                _gameCache.Set(GetGameKey(gameState.lobby), gameState, GetDefaultOptions());

                // Init and free up the one space
                var newLock = new Semaphore(0, 1);
                newLock.Release();
                _lockCache.Set(GetGameKey(gameState.lobby), newLock, GetDefaultOptions());
            }

            // Commence the game
            gameState.status = GameStatus.WarmUp;

            // set the game again, after updating it
            _gameCache.Set(GetGameKey(gameState.lobby), gameState, GetDefaultOptions());

            return roomKey;
        }

        public Guid[]? GetPlayerIds(string groupName)
        {
            var roomKey = GetGameKey(groupName);

            if (_gameCache.TryGetValue(roomKey, out GameState roomGame))
            {
                return roomGame.players.Select(pl => pl.uuid).ToArray();
            }

            return null;
        }

        private MemoryCacheEntryOptions GetDefaultOptions()
        {
            return new MemoryCacheEntryOptions()
            {
                // A game lobby will automatically expire (and be marked for removal from in-memory cache)
                //  4 hours after being created 
                AbsoluteExpiration = DateTimeOffset.UtcNow + TimeSpan.FromHours(4),
                // This size is unitless, but corresponds to the same unitless SizeLimit in MemoryCacheOptions above
                Size = 1,
                // A game lobby will also automatically expire if idle for 15 minutes.
                SlidingExpiration = TimeSpan.FromMinutes(15),
            };
        }

        public ConcurrentGameState? GetGameState(string key)
        {
            Semaphore cachedLock;

            lock (_lockObject)
            {
                if (!_lockCache.TryGetValue(key, out cachedLock))
                {
                    return null;
                }
            }

            cachedLock.WaitOne();

            if (_gameCache.TryGetValue(key, out GameState game))
            {
                return new ConcurrentGameState(game, cachedLock);
            }

            return null;
        }

        public void UpdateGameState(string key, ConcurrentGameState game)
        {
            _gameCache.Set(key, game.GameState, GetDefaultOptions());
        }

        public string GetGameKey(string roomKey)
        {
            return $"{roomKey}";
        }

        public int GetGameCount()
        {
            return _gameCache.Count;
        }
    }
}
