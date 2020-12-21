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
            var game = await _gameService.FindGameAsync(gameId);

            var player = game.Players.ToList().FirstOrDefault(x => x.Id == playerId);

            if (player != null)
            {
                //_logger.LogInformation("Player with a Player Id of " + playerId + " was found.  Marking question answered.");

                game.AnswerQuestion(player, questionId, answer);

                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("QuestionAnswered", game, player);
            }
            else
            {
                _logger.LogError("Player not found!");
            }
        }

        public async Task SubmitCorrectGuess(Guid gameId, int questionId, Guid playerId, int guess) 
        {
            var game = await _gameService.FindGameAsync(gameId);
            
            // _logger.LogInformation($"Recording a guess of {guess} for {playerId}");

            game = game.RecordGuess(questionId, playerId, guess);

            // _logger.LogInformation($"Guess recorded - {game.AnsweredQuestions.Where(x => x.Guess > 0).Count()} think more than zero");
        }

        public async Task AskQuestion(Guid gameId, string category)
        {
            var game = await _gameService.FindGameAsync(gameId);

            var question = _wordPlay.GetRandomQuestion(category);
            
            //_logger.LogInformation("Using category " + category);
            //_logger.LogInformation("Randomly chose question " + question.Id);

            // todo: double check, it should be handled in QuestionFinished!
            if (game.Questions.Count == game.Players.Count)
            {
                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("GameOver", game);
            }
            
            game = await _gameService.AddQuestionAsync(gameId, question);

            var player = game.GetNextPlayer();
            
            await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("QuestionReady", game, question, player);
        }

        public async Task QuestionFinished(Guid gameId, int questionId, int score)
        {
            var game = await _gameService.FindGameAsync(gameId);
            
            game = game.RecordScore(questionId, score).RecordGuesses(questionId).ResetCurrentPlayerAnswers();
            
            var roundEnded = false;

            if (game.AskedEnoughQuestionsForRound())
            {
                roundEnded = true;
                game = _gameService.StartNextRound(gameId); // game.NextRound();

                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("RoundOver", game);                
            }

            //_logger.LogInformation(roundEnded + " round ended");
            //_logger.LogInformation(game.State + " game state");
            
            if (game.State == GameState.Ended)
            {
                game = _gameService.EndGame(gameId);

                // _logger.LogInformation("Hey, its Game Over!");
                
                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("GameOver", game);
            }
            else if (!roundEnded)
            {                
                //_logger.LogInformation("Tally guesses (and score?) Proceeding to next question");
                await Clients.Caller.SendAsync("ProceedToNextQuestion", game);                        
            }            
        }

        public async Task UpdateConnectionId(Guid gameId, Guid playerId)
        {
            var game = await _gameService.FindGameAsync(gameId);

            var player = game.UpdatePlayerConnection(playerId, Context.ConnectionId);
            
            await Clients.Client(Context.ConnectionId).SendAsync("PlayerUpdated", player);
        }

        public async Task UpdateGameConnectionId(Guid gameId)
        {
            var game = await _gameService.FindGameAsync(gameId);

            _logger.LogInformation($"Setting game connection id {Context.ConnectionId}");

            game = game.UpdateGameConnection(gameId, Context.ConnectionId);

            await Clients.Client(Context.ConnectionId).SendAsync("GameConnected", game);
        }

        public async Task AnswersFinished(Guid gameId)
        {
            var game = await _gameService.FindGameAsync(gameId);
            
            game.ShuffleAnswers();

            var phrase = _phrases.GetRandomAnswersFinishedPhrase(game.CurrentPlayer.Username, game.Voice) as SpeechSynthesisUtterance;
            
            _logger.LogInformation("AnswersFinished, grabbed phrase for " + game.CurrentPlayer.Username);

            await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("AnswersFinished", game, phrase);
        }

        public async Task QuestionCompletedAnswer(Guid gameId, bool timeExpired, List<AnsweredQuestion> currentPlayerAnswers)
        {
            // todo: should calculate Game scores and Answers here
            
            var game = await _gameService.FindGameAsync(gameId);

            var rand = new Random();            
            game.CurrentPlayerAnswers = currentPlayerAnswers.OrderBy(x => rand.Next()).ToList();
            
            if (!timeExpired)
            {
                // Console.WriteLine("with time on the clock");
                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("QuestionSummaryStarted", timeExpired, game);
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

        public async Task ResetGame(Guid gameId)
        {
            var game = await _gameService.FindGameAsync(gameId);

            var previousPlayers = game.GameAndAllPlayerConnections();

            game = _gameService.ResetGame(gameId);

            await Clients.Clients(previousPlayers).SendAsync("GameReset", game);
        }
        
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}