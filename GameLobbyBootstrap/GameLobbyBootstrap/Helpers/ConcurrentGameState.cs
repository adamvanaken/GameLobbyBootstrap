using MyApp.Models;
using System;
using System.Threading;

namespace MyApp.Helpers
{
    public class ConcurrentGameState : IDisposable
    {
        private Semaphore _spinLock;
        public GameState GameState;

        public ConcurrentGameState(GameState gameState, Semaphore spinLock)
        {
            this._spinLock = spinLock;
            this.GameState = gameState;
        }

        public void Dispose()
        {
            this._spinLock.Release();
        }
    }
}
