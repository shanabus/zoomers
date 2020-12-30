using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Toolbelt.Blazor.SpeechSynthesis;
using ZoomersClient.Server.Services;
using ZoomersClient.Shared.Models;
using ZoomersClient.Shared.Models.DTOs;
using ZoomersClient.Shared.Models.Enums;
using ZoomersClient.Shared.Services;

namespace ZoomersClient.Server.Hubs
{
    public class WordPlayHub : Hub, IGameTypeHub
    {
        private ILogger<WordPlayHub> _logger { get; set; }
        private GameService _gameService { get; set; }
        private Phrases _phrases { get; set; }
        private IMapper _mapper { get; set; }

        public WordPlayHub(ILogger<WordPlayHub> logger, GameService gameService, Phrases phrases, IMapper mapper)
        {
            _logger = logger;
            _gameService = gameService;
            _phrases = phrases;
            _mapper = mapper;
        }

        public async Task AnswerQuestion(Guid gameId, int questionId, Guid playerId, string answer)
        {
            var game = await _gameService.FindGameAsync(gameId);

            var player = game.Players.ToList().FirstOrDefault(x => x.Id == playerId);

            if (player != null)
            {
                //_logger.LogInformation("Player with a Player Id of " + playerId + " was found.  Marking question answered.");

                game = await _gameService.AnswerQuestionAsync(gameId, player, questionId, answer);

                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("QuestionAnswered", game, player);
            }
            else
            {
                _logger.LogError("Player not found!");
            }
        }

        public async Task SubmitCorrectGuess(Guid gameId, int questionId, Guid playerId, int? guess) 
        {            
            await _gameService.RecordGuessAsync(gameId, questionId, playerId, guess.Value);    
            Console.WriteLine("A guess should have been logged");                    
        }

        public async Task AskQuestion(Guid gameId, string category)
        {
            var game = await _gameService.FindGameAsync(gameId);

            var questionBase = _gameService.GetRandomQuestion(game, category);           

            var question = _mapper.Map<GameQuestion>(questionBase);

            game = await _gameService.AddQuestionAsync(gameId, question);
            
            Console.WriteLine("Sending QuestionReady");

            var q = _mapper.Map<GameQuestionDto>(question);
            
            await Clients.All.SendAsync("QuestionReady", game, q, game.CurrentPlayer);
        }

        public async Task QuestionFinished(Guid gameId, int questionId, int score)
        {
            var game = await _gameService.RecordScoresAsync(gameId, questionId, score);
                        
            var roundEnded = false;

            if (game.AskedEnoughQuestionsForRound())
            {
                roundEnded = true;
                game = await _gameService.StartNextRoundAsync(gameId); 

                if (game.State != GameState.Ended)
                {
                    await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("RoundOver", game);                
                }
            }
            
            if (game.State == GameState.Ended)
            {
                Console.WriteLine("State says Game Ended");
                game = await _gameService.EndGameAsync(gameId);

                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("GameOver", game);
            }
            else if (!roundEnded)
            {                
                await Clients.Caller.SendAsync("ProceedToNextQuestion", game);                        
            }            
        }

        public async Task UpdatePlayerConnectionId(Guid gameId, Guid playerId)
        {
            try{
                var game = await _gameService.UpdatePlayerConnectionId(gameId, playerId, Context.ConnectionId);

                await Clients.Client(Context.ConnectionId).SendAsync("PlayerUpdated", game);
            }
            catch(Exception e)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ConnectionError", e.Message);
            }
            
        }

        public async Task UpdateGameConnectionId(Guid gameId)
        {
            var game = await _gameService.UpdateGameConnection(gameId, Context.ConnectionId);

            // _logger.LogInformation($"Setting game connection id {Context.ConnectionId}");

            await Clients.Client(Context.ConnectionId).SendAsync("GameConnected", game);
        }

        public async Task AnswersFinished(Guid gameId)
        {
            var game = await _gameService.AnswersFinishedAsync(gameId);
            
            game.ShuffleAnswers();

            var phrase = _phrases.GetRandomAnswersFinishedPhrase(game.CurrentPlayer.Username, game.Voice) as SpeechSynthesisUtterance;
            
            await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("AnswersFinished", game, phrase);
        }

        public async Task QuestionCompletedAnswer(Guid gameId, bool timeExpired, List<AnsweredQuestionDto> xurrentPlayerAnswers)
        {            
            var game = await _gameService.QuestionCompletedAnswerAsync(gameId, xurrentPlayerAnswers);
            
            await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("QuestionSummaryStarted", timeExpired, game);               
        }

        public async Task SendReaction(Guid gameId, PlayerDto fromPlayer, PlayerDto toPlayer, AnswerReaction reaction)
        {   
            if (fromPlayer.Id != toPlayer.Id)
            {
                var game = await _gameService.AddAudienceReactionAsync(gameId, fromPlayer, toPlayer, reaction); 
                
                await Clients.Clients(game.GameAndPlayerConnections(toPlayer)).SendAsync("ReactionReceived", fromPlayer, toPlayer, reaction);
            }            
        }

        public async Task ResetGame(Guid gameId)
        {
            var game = await _gameService.FindGameAsync(gameId);

            var previousPlayers = game.GameAndAllPlayerConnections();

            game = await _gameService.ResetGameAsync(gameId);

            await Clients.Clients(previousPlayers).SendAsync("GameReset", game);
        }
        
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}