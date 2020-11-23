using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Toolbelt.Blazor.SpeechSynthesis;
using ZoomersClient.Server.Services;
using ZoomersClient.Shared.Services;

namespace ZoomersClient.Server.Hubs
{
    public class WordPlayHub : Hub, IGameTypeHub
    {
        private WordPlay _wordPlay { get; set; }
        private ILogger<WordPlayHub> _logger { get; set; }
        private GameService _gameService { get; set; }
        private Phrases _phrases { get; set; }

        public WordPlayHub(ILogger<WordPlayHub> logger, WordPlay wordplay, GameService gameService, Phrases phrases)
        {
            _logger = logger;
            _wordPlay = wordplay;
            _gameService = gameService;
            _phrases = phrases;
        }

        public async Task GetPlayerOrder(Guid gameId)
        {
            var game = _gameService.FindGame(gameId);

            game.ShufflePlayerOrder();

            await Clients.All.SendAsync("PlayersShuffled", game); 
        }
        
        public async Task AnswerQuestion(Guid gameId, int questionId, Guid playerId, string answer)
        {
            _logger.LogInformation("Processing an Answer! " + answer);

            var game = _gameService.FindGame(gameId);

            var player = game.Players.ToList().FirstOrDefault(x => x.Id == playerId);

            if (player != null)
            {
                _logger.LogInformation("Player with ConnId of " + playerId + " was found.  Marking question answered.");

                game.AnswerQuestion(player, questionId, answer);

                await Clients.All.SendAsync("QuestionAnswered", player);
            }
            else
            {
                _logger.LogError("Player not found!");
            }
        }

        public async Task AskQuestion(Guid gameId)
        {
            var question = _wordPlay.GetRandomQuestion(null);

            _logger.LogInformation("Randomly chose question " + question.Id);

            _gameService.AddQuestion(gameId, question);

            var player = _gameService.GetNextPlayer(gameId);
            
            await Clients.All.SendAsync("QuestionReady", question, player);
        }

        public Task QuestionFinished(Guid gameId, int questionId)
        {
            var game = _gameService.FindGame(gameId);

            // total scores?  let player whose turn it is answer?

            throw new System.NotImplementedException();
        }

        public async Task UpdateConnectionId(Guid gameId, Guid playerId)
        {
            var game = _gameService.FindGame(gameId);

            _logger.LogInformation($"Setting player connection id {Context.ConnectionId}");

            game.UpdatePlayerConnection(playerId, Context.ConnectionId);
        }

        public async Task AnswersFinished(Guid gameId, string username)
        {
            var game = _gameService.FindGame(gameId);

            var phrase = _phrases.GetRandomAnswersFinishedPhrase(username, game.Voice) as SpeechSynthesisUtterance;
            
            await Clients.All.SendAsync("AnswersFinished", game, phrase);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        
    }
}