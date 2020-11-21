using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZoomersClient.Shared.Services;
using ZoomersClient.Shared.Models;
using System.Linq;
using Net.Codecrete.QrCodeGenerator;
using System.Drawing.Imaging;
using System.IO;

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
        public async Task<IActionResult> GetGame([FromRoute]Guid id)
        {
            var game = _gameService.FindGame(id);

            foreach(var p in game.Players)
            {
                _logger.LogInformation("Games has player: " + p.Username);
            }

            return Ok(game);
        }

        [HttpGet("{id}/players")]
        public IEnumerable<string> GetPlayers([FromRoute]Guid id)
        {
            var game = _gameService.FindGame(id);

            foreach(var p in game.Players)
            {
                _logger.LogInformation("Still has player " + p.Username);
            }

            return game.Players.Select(x => x.Username);
        }

        [HttpPost]
        public Game Post([FromBody] string name)
        {
            var game = new Game(name);
            _gameService.CreateGame(game);
            return game;
        }

        [HttpGet("{id}/qrcode")]
        public IActionResult GetQrCode([FromQuery]Guid id)
        {
            var qr = QrCode.EncodeText("http://teamsievers.ddns.net:5000/join", QrCode.Ecc.Medium);
            using (var bitmap = qr.ToBitmap(10, 4))
            {
                // bitmap.Save("qr-code.png", ImageFormat.Png);

                MemoryStream ms = new MemoryStream();  
                bitmap.Save(ms, ImageFormat.Png); 

                return File(ms.ToArray(), "image/png");
            }
        } 
    }
}
