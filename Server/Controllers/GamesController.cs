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
using System.Threading.Tasks;

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
        public async Task<Game[]> Get()
        {
            var games = await _gameService.AllGamesAsync();

            return games;
        }

        [HttpGet("{id}")]
        public async Task<Game> GetGame([FromRoute]Guid id)
        {
            var game = await _gameService.FindGameAsync(id);

            return game;
        }

        [HttpGet("{id}/players")]
        public async Task<IEnumerable<string>> GetPlayers([FromRoute]Guid id)
        {
            var game = await _gameService.FindGameAsync(id);

            return game.Players.Select(x => x.Username);
        }

        [HttpPost]
        public async Task<Game> Post([FromBody] CreateGameDto dto)
        {
            var game = new Game(dto.Name, dto.Voice, dto.Rounds);
            
            return await _gameService.CreateGameAsync(game);            
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

        [HttpDelete("{id}")]
        public async Task Delete([FromRoute]Guid id)
        {
            await _gameService.DeleteAsync(id);
        }
    }
}
