using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ZoomersClient.Server.Services;
using ZoomersClient.Shared.Services;

namespace ZoomersClient.Server.Hubs
{
    public class WordPlayHub : Hub, IGameTypeHub
    {
        private WordPlay _wordPlay { get; set; }
        private ILogger<WordPlayHub> _logger { get; set; }
        private GameService _gameService { get; set; }
        
        public WordPlayHub(ILogger<WordPlayHub> logger, WordPlay wordplay, GameService gameService)
        {
            _logger = logger;
            _wordPlay = wordplay;
            _gameService = gameService;
        }
        
        public async Task AnswerQuestion(Guid id, int questionId, string answer)
        {
            _logger.LogInformation("Processing an Answer! " + answer);

            var game = _gameService.FindGame(id);

            var player = game.Players.ToList().FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            if (player != null)
            {
                _logger.LogInformation("Player with ConnId of " + player.ConnectionId + " was found.  Marking question answered.");

                game.AnswerQuestion(player, questionId, answer);

                await Clients.All.SendAsync("QuestionAnswered", player);
            }
            else
            {
                _logger.LogError("Player not found!");
            }
        }

        public async Task AskQuestion(Guid id)
        {
            var q = _wordPlay.GetRandomQuestion(null);

            _logger.LogInformation("Randomly chose question " + q.Id);

            _gameService.AddQuestion(id, q);
            
            await Clients.All.SendAsync("QuestionReady", q);
        }

        public Task NextQuestion(Guid id)
        {
            var game = _gameService.FindGame(id);

            // total scores?  let player whose turn it is answer?

            throw new System.NotImplementedException();
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}