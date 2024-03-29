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
        private readonly GameConfiguration _gameConfiguration;
        private readonly ILogger<GamesController> _logger;

        public GamesController(ILogger<GamesController> logger, GameService gameService, Phrases phrases, GameConfiguration gameConfiguration)
        {
            _logger = logger;
            _gameService = gameService;
            _phrases = phrases;
            _gameConfiguration = gameConfiguration;
        }

        [HttpGet]
        public IEnumerable<GameDto> Get()
        {
            var games = _gameService.AllGames();
            return games;
        }

        [HttpGet("{id}")]
        public async Task<GameDto> GetGame([FromRoute]Guid id)
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
        public async Task<GameDto> Post([FromBody] CreateGameDto dto)
        {
            Console.WriteLine("Creating game - " + dto.Name);
            var game = new Game(dto.Name, dto.Voice, dto.Rounds);
            var gameresult = await _gameService.CreateGameAsync(game);
            return gameresult;
        }

        [HttpGet("{id}/qrcode")]
        public IActionResult GetQrCode([FromQuery]Guid id)
        {
            var qr = QrCode.EncodeText(_gameConfiguration.HostUrl, QrCode.Ecc.Medium);
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
        public async Task<ActionResult> DeleteAsync([FromRoute] Guid id)
        {
            await _gameService.DeleteAsync(id);
            
            return NoContent();        
        }
    }
}
