using GameLobbyBootstrap.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLobbyBootstrap.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly IGameStateProvider _gameStateProvider;

        public GameController(ILogger<GameController> logger, IGameStateProvider gameStateProvider)
        {
            _logger = logger;
            _gameStateProvider = gameStateProvider;
        }

        [HttpGet]
        [Route("gameCount")]
        public ActionResult<int> GetGameCount()
        {
            return Ok(_gameStateProvider.GetGameCount());
        }
    }
}