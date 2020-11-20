using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZoomersClient.Shared;
using ZoomersClient.Shared.Services;
using ZoomersClient.Shared.Models;

namespace ZoomersClient.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly GameService _gameService;

        private readonly ILogger<GamesController> _logger;

        public GamesController(ILogger<GamesController> logger, GameService gameService)
        {
            _logger = logger;
            _gameService = gameService;
        }

        [HttpGet]
        public IEnumerable<Game> Get()
        {
            var games = _gameService.Games;
            return games;
        }

        [HttpGet("{id}")]
        public Game GetGame([FromRoute]Guid id)
        {
            var game = _gameService.FindGame(id);
            return game;
        }

        [HttpPost]
        public Game Post([FromBody] string name)
        {
            var game = new Game(name);
            _gameService.CreateGame(game);
            return game;
        }
    }
}
