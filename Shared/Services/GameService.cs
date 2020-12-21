using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ZoomersClient.Shared.Data;
using ZoomersClient.Shared.Exceptions;
using ZoomersClient.Shared.Models;
using ZoomersClient.Shared.Models.Enums;

namespace ZoomersClient.Shared.Services
{
    public class GameService
    {        
        public List<Game> Games { get; set; }
        private ILogger<GameService> _logger { get; set; }
        private GameRepository _gameRepository { get; set; }

        public GameService(ILogger<GameService> logger, GameRepository gameRepository)
        {
            _logger = logger;
            _gameRepository = gameRepository;

            Games = new List<Game>();            
        }

        public async Task<Game> SeedDefaultGame()
        {
            Games = new List<Game>();
            // https://postimg.cc/gallery/hxv1nzD/4752bea0
            var defaultGame = new Game("SWB Game", "en-GB", 2);
            defaultGame.Id = Guid.Parse("5b05a3a6-7665-47dd-b515-03372211a95e");
            Games.Add(defaultGame);

            return await _gameRepository.SaveAsync(defaultGame);
        }

        public async Task<Game> FindGameAsync(Guid id)
        {
            // var game = Games.FirstOrDefault(x => x.Id == id);
            var game = await _gameRepository.GetAsync(id);

            return game; 
        }

        public Game FindGame(List<PartyIcon> party)
        {
            var game = Games.FirstOrDefault(x => x.Party[0] == party[0] && x.Party[1] == party[1] && x.Party[2] == party[2]);

            return game; 
        }

        public async Task<Game> CreateGameAsync(Game game)
        {
            // Games.Add(game);
            return await _gameRepository.SaveAsync(game);
        }

        public Game JoinGame(Guid id, Player player)
        {
            var game = Games.FirstOrDefault(x => x.Id == id);

            if (game != null)
            {
                var existingPlayer = game.Players.Where(x => x.Id == player.Id);
                if (existingPlayer.Any())
                {
                    _logger.LogInformation("Found an existing player with same Id.");                    
                }
                else
                {
                    game.Players.Add(player);                
                }
                
            }

            return game;
        }

        public async Task<Game> AddQuestionAsync(Guid id, WordPlayQuestion q)
        {
            var game = await FindGameAsync(id);

            if (game != null) {
                // _logger.LogInformation(q.Question + " was just added");
                
                game.ResetAnswers()
                    .Questions.Add(q as QuestionBase);

                // _logger.LogInformation(game.Questions.Count + " questions total");
            }

            return game;
        }

        public void UpdateConnectionId(Guid id, string connectionId)
        {
            var game = Games.FirstOrDefault(x => x.Id == id);

            if (game != null)
            {
                game.ConnectionId = connectionId;
            }
        }

        public Player GetNextPlayer(Guid gameId)
        {
            var game = Games.FirstOrDefault(x => x.Id == gameId);

            if (game != null)
            {
                return game.GetNextPlayer();
            }
            
            throw new GameNotFoundException();
        }

        public Game EndGame(Guid gameId)
        {
            var game = Games.FirstOrDefault(x => x.Id == gameId);

            if (game != null)
            {
                return game.EndGame();
            }
            
            return game;
        }

        public Game StartGame(Guid gameId)
        {
            var game = Games.FirstOrDefault(x => x.Id == gameId);

            if (game != null)
            {
                game.StartGame();
            }

            return game;
        }

        public Game StartNextRound(Guid gameId)
        {
            var game = Games.FirstOrDefault(x => x.Id == gameId);

            if (game != null)
            {
                game.NextRound();
            }
            
            return game;
        }

        public Game AddAudienceReaction(Guid gameId, Player fromPlayer, Player toPlayer, AnswerReaction reaction)
        {
            var game = Games.FirstOrDefault(x => x.Id == gameId);

            if (game != null)
            {
                game.AddReaction(fromPlayer, toPlayer, reaction);
            }
            
            return game;
        }

        public Game ResetGame(Guid gameId)
        {
            var game = Games.FirstOrDefault(x => x.Id == gameId);

            if (game != null)
            {
                game.ResetGame();
            }
            
            return game;
        }

        public async Task DeleteAsync(Guid gameId)
        {
            await _gameRepository.DeleteAsync(gameId);
        }
    }
}