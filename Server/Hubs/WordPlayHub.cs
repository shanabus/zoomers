using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Toolbelt.Blazor.SpeechSynthesis;
using ZoomersClient.Server.Services;
using ZoomersClient.Shared.Models;
using ZoomersClient.Shared.Models.Enums;
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
            var game = _gameService.FindGame(gameId);

            var player = game.Players.ToList().FirstOrDefault(x => x.Id == playerId);

            if (player != null)
            {
                _logger.LogInformation("Player with a Player Id of " + playerId + " was found.  Marking question answered.");

                game.AnswerQuestion(player, questionId, answer);

                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("QuestionAnswered", game, player);
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
                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("GameOver", game);
            }
            
            game = _gameService.AddQuestion(gameId, question);

            var player = game.GetNextPlayer();
            
            await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("QuestionReady", game, question, player);
        }

        public async Task QuestionFinished(Guid gameId, int questionId, int score)
        {
            var game = _gameService.FindGame(gameId);

            // total scores?  let player whose turn it is answer?
            game = game.RecordScore(questionId, score);
            
            var roundEnded = false;

            if (game.Questions.Count == game.Players.Count)
            {
                roundEnded = true;
                game = game.NextRound();

                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("RoundOver", game);                
            }

            _logger.LogInformation(roundEnded + " round ended");
            _logger.LogInformation(game.State + " game state");
            
            if (game.State == GameState.Ended)
            {
                game = _gameService.EndGame(gameId);

                _logger.LogInformation("Hey, its Game Over!");
                
                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("GameOver", game);
            }
            else if (!roundEnded)
            {
                _logger.LogInformation("Proceeding to next question");
                await Clients.Caller.SendAsync("ProceedToNextQuestion", game);                        
            }            
        }

        public async Task UpdateConnectionId(Guid gameId, Guid playerId)
        {
            var game = _gameService.FindGame(gameId);

            var player = game.UpdatePlayerConnection(playerId, Context.ConnectionId);
            
            await Clients.Client(Context.ConnectionId).SendAsync("PlayerUpdated", player);
        }

        public async Task UpdateGameConnectionId(Guid gameId)
        {
            var game = _gameService.FindGame(gameId);

            _logger.LogInformation($"Setting game connection id {Context.ConnectionId}");

            game = game.UpdateGameConnection(gameId, Context.ConnectionId);

            await Clients.Client(Context.ConnectionId).SendAsync("GameConnected", game);
        }

        public async Task AnswersFinished(Guid gameId)
        {
            var game = _gameService.FindGame(gameId).ShuffleAnswers();

            var phrase = _phrases.GetRandomAnswersFinishedPhrase(game.CurrentPlayer.Username, game.Voice) as SpeechSynthesisUtterance;
            
            _logger.LogInformation("AnswersFinished on WordPlayHub was called");

            await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("AnswersFinished", game, phrase);
        }

        public async Task QuestionCompletedAnswer(Guid gameId, bool timeExpired, List<AnsweredQuestion> currentPlayerAnswers)
        {
            // todo: should calculate Game scores and Answers here
            
            var game = _gameService.FindGame(gameId);

            var rand = new Random();
            var randomList = currentPlayerAnswers.OrderBy(x => rand.Next()).ToList();

            // needs to be different!
            if (!timeExpired)
            {
                // Console.WriteLine("with time on the clock");
                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("QuestionSummaryStarted", timeExpired, randomList);
            }            
            else {
                Console.WriteLine("Time expired for current player?");
            }
        }

        public async Task SendReaction(Guid gameId, Player fromPlayer, Player toPlayer, AnswerReaction reaction)
        {   
            if (fromPlayer.Id != toPlayer.Id)
            {
                var game = _gameService.AddAudienceReaction(gameId, fromPlayer, toPlayer, reaction); 
                
                await Clients.Clients(game.GameAndPlayerConnections(toPlayer)).SendAsync("ReactionReceived", fromPlayer, toPlayer, reaction);
            }            
        }

        public async Task ResetDefaultGame(Guid gameId)
        {
            var game = _gameService.ResetDefaultGame();

            await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("GameReset");
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}