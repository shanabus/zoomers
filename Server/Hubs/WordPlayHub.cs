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
        private WordPlay _wordPlay { get; set; }
        private ILogger<WordPlayHub> _logger { get; set; }
        private GameService _gameService { get; set; }
        private Phrases _phrases { get; set; }
        private IMapper _mapper { get; set; }

        public WordPlayHub(ILogger<WordPlayHub> logger, WordPlay wordplay, GameService gameService, Phrases phrases, IMapper mapper)
        {
            _logger = logger;
            _wordPlay = wordplay;
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

        public async Task SubmitCorrectGuess(Guid gameId, int questionId, Guid playerId, int guess) {
            
            await _gameService.RecordGuessAsync(gameId, questionId, playerId, guess);            
            
        }

        public async Task AskQuestion(Guid gameId, string category)
        {
            var game = await _gameService.FindGameAsync(gameId);

            var question = _wordPlay.GetRandomQuestion(category);
            
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

                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("RoundOver", game);                
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
            var game = await _gameService.UpdatePlayerConnectionId(gameId, playerId, Context.ConnectionId);
            
            await Clients.Client(Context.ConnectionId).SendAsync("PlayerUpdated", game);
        }

        public async Task UpdateGameConnectionId(Guid gameId)
        {
            var game = await _gameService.UpdateGameConnection(gameId, Context.ConnectionId);

            // _logger.LogInformation($"Setting game connection id {Context.ConnectionId}");

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

        public async Task QuestionCompletedAnswer(Guid gameId, bool timeExpired, List<AnsweredQuestionDto> xurrentPlayerAnswers)
        {
            // todo: should calculate Game scores and Answers here
            
            var game = await _gameService.FindGameAsync(gameId);

            var rand = new Random();            
            Console.WriteLine("FIX THIS NOW! SAVE AND CHANGE VAR NAMES");
            game.AnsweredQuestions = xurrentPlayerAnswers.OrderBy(x => rand.Next()).ToList();
            
            if (!timeExpired)
            {
                // Console.WriteLine("with time on the clock");
                await Clients.Clients(game.GameAndAllPlayerConnections()).SendAsync("QuestionSummaryStarted", timeExpired, game);
            }            
            else {
                Console.WriteLine("Time expired for current player?");
            }
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