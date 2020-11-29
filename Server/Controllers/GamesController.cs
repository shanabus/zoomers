using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZoomersClient.Shared.Services;
using ZoomersClient.Shared.Models;
using System.Linq;
using Net.Codecrete.QrCodeGenerator;
using System.Drawing.Imaging;
using System.IO;
using ZoomersClient.Shared.Models.DTOs;
using ZoomersClient.Server.Services;

namespace ZoomersClient.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly GameService _gameService;
        private readonly Phrases _phrases;
        private readonly ILogger<GamesController> _logger;

        public GamesController(ILogger<GamesController> logger, GameService gameService, Phrases phrases)
        {
            _logger = logger;
            _gameService = gameService;
            _phrases = phrases;
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

        [HttpGet("{id}/players")]
        public IEnumerable<string> GetPlayers([FromRoute]Guid id)
        {
            var game = _gameService.FindGame(id);

            return game.Players.Select(x => x.Username);
        }

        [HttpPost]
        public Game Post([FromBody] CreateGameDto dto)
        {
            var game = new Game(dto.Name, dto.Voice, dto.Rounds);
            _gameService.CreateGame(game);
            return game;
        }

        [HttpGet("{id}/qrcode")]
        public IActionResult GetQrCode([FromQuery]Guid id)
        {
            var qr = QrCode.EncodeText("http://teamsievers.ddns.net:5000/join", QrCode.Ecc.Medium);
            using (var bitmap = qr.ToBitmap(10, 4))
            {
                MemoryStream ms = new MemoryStream();  
                bitmap.Save(ms, ImageFormat.Png); 

                return File(ms.ToArray(), "image/png");
            }
        } 

        [HttpGet("{id}/playerjoinedphrases")]
        public SpokenPhrase GetPlayerJoinedPhrases([FromQuery]string username, string voice)
        {
            var phrase = _phrases.GetRandomPlayerJoinedPhrase(username, voice);
            
            return phrase;        
        }
    }
}
