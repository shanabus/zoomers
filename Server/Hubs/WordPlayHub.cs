using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Toolbelt.Blazor.SpeechSynthesis;
using ZoomersClient.Server.Services;
using ZoomersClient.Shared.Models;
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

        public async Task AnswerQuestion(Guid gameId, int questionId, Guid playerId, string answer)
        {
            _logger.LogInformation("Processing an Answer! " + answer);

            var game = _gameService.FindGame(gameId);

            var player = game.Players.ToList().FirstOrDefault(x => x.Id == playerId);

            if (player != null)
            {
                _logger.LogInformation("Player with ConnId of " + playerId + " was found.  Marking question answered.");

                game.AnswerQuestion(player, questionId, answer);

                await Clients.All.SendAsync("QuestionAnswered", game, player);
            }
            else
            {
                _logger.LogError("Player not found!");
            }
        }

        public async Task AskQuestion(Guid gameId)
        {
            var game = _gameService.FindGame(gameId);

            var question = _wordPlay.GetRandomQuestion(null);

            _logger.LogInformation("Randomly chose question " + question.Id);

            // todo: double check, it should be handled in QuestionFinished!
            if (game.Questions.Count == game.Players.Count)
            {
                await Clients.All.SendAsync("GameOver", game);
            }
            
            game = _gameService.AddQuestion(gameId, question);

            var player = game.GetNextPlayer();
            
            await Clients.All.SendAsync("QuestionReady", game, question, player);
        }

        public async Task QuestionFinished(Guid gameId, int questionId, int score)
        {
            var game = _gameService.FindGame(gameId);

            // total scores?  let player whose turn it is answer?
            game.RecordScore(questionId, score);

            if (game.Questions.Count == game.Players.Count)
            {
                game = _gameService.EndGame(gameId);

                _logger.LogInformation("Hey, its Game Over!");
                
                await Clients.All.SendAsync("GameOver", game);
            }
            else
            {
                _logger.LogInformation("Proceeding to next question");
                await Clients.Caller.SendAsync("ProceedToNextQuestion", game);
            }            
        }

        public void UpdateConnectionId(Guid gameId, Guid playerId)
        {
            var game = _gameService.FindGame(gameId);

            _logger.LogInformation($"Setting player connection id {Context.ConnectionId}");

            game.UpdatePlayerConnection(playerId, Context.ConnectionId);
        }

        public async Task AnswersFinished(Guid gameId, string username)
        {
            var game = _gameService.FindGame(gameId);

            var phrase = _phrases.GetRandomAnswersFinishedPhrase(username, game.Voice) as SpeechSynthesisUtterance;
            
            _logger.LogInformation("AnswersFinished on WordPlayHub was called");

            await Clients.All.SendAsync("AnswersFinished", game, phrase);
        }

        public async Task QuestionCompletedAnswer(Guid gameId, bool timeExpired, List<AnsweredQuestion> currentPlayerAnswers)
        {
            // todo: should calculate Game scores and Answers here
            Console.WriteLine("Received player answers");

            var rand = new Random();
            var randomList = currentPlayerAnswers.OrderBy(x => rand.Next()).ToList();

            // needs to be different!
            if (!timeExpired)
            {
                Console.WriteLine("with time on the clock");
                await Clients.All.SendAsync("QuestionSummaryStarted", timeExpired, randomList);
            }            
            else {
                Console.WriteLine("Time expired for current player?");
            }
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}