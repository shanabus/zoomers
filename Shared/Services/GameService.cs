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
        private ILogger<GameService> _logger { get; set; }
        private ApplicationDBContext _database { get; set; }

        public GameService(ILogger<GameService> logger, ApplicationDBContext database)
        {
            _logger = logger;
            _database = database;
            
            // SeedDefaultGame();
        }

        public List<Game> AllGames()
        {
            return _database.Games.ToList();
        }

        // public Game SeedDefaultGame()
        // {
        //     if 
        //     // https://postimg.cc/gallery/hxv1nzD/4752bea0
        //     var defaultGame = new Game("SWB Game", "en-GB", 2);
        //     defaultGame.Id = Guid.Parse("5b05a3a6-7665-47dd-b515-03372211a95e");
        //     Games.Add(defaultGame);

        //     return defaultGame;
        // }

        public Game FindGame(Guid id)
        {
            Console.WriteLine("Looking for game...");
            var game = _database.Games.FirstOrDefault(x => x.Id == id);
            
            return game; 
        }

        public Game FindGame(List<string> party)
        {
            var game = _database.Games.FirstOrDefault(x => x.Party == string.Join('|', party));

            return game; 
        }

        public async Task<Game> CreateGameAsync(Game game)
        {
            await _database.Games.AddAsync(game);
            await _database.SaveChangesAsync();

            return game;
        }

        public Game JoinGame(Guid id, Player player)
        {
            var game = _database.Games.FirstOrDefault(x => x.Id == id);

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

        public Game AddQuestion(Guid id, QuestionBase q)
        {
            var game = FindGame(id);

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
            var game = _database.Games.FirstOrDefault(x => x.Id == id);

            if (game != null)
            {
                game.ConnectionId = connectionId;
            }
        }

        public Player GetNextPlayer(Guid gameId)
        {
            var game = _database.Games.FirstOrDefault(x => x.Id == gameId);

            if (game != null)
            {
                return game.GetNextPlayer();
            }
            
            throw new GameNotFoundException();
        }

        public Game EndGame(Guid gameId)
        {
            var game = _database.Games.FirstOrDefault(x => x.Id == gameId);

            if (game != null)
            {
                return game.EndGame();
            }
            
            return game;
        }

        public Game StartGame(Guid gameId)
        {
            var game = _database.Games.FirstOrDefault(x => x.Id == gameId);

            if (game != null)
            {
                game.StartGame();
            }

            return game;
        }

        public Game StartNextRound(Guid gameId)
        {
            var game = _database.Games.FirstOrDefault(x => x.Id == gameId);

            if (game != null)
            {
                game.NextRound();
            }
            
            return game;
        }

        public Game AddAudienceReaction(Guid gameId, Player fromPlayer, Player toPlayer, AnswerReaction reaction)
        {
            var game = _database.Games.FirstOrDefault(x => x.Id == gameId);

            if (game != null)
            {
                game.AddReaction(fromPlayer, toPlayer, reaction);
            }
            
            return game;
        }

        public Game ResetGame(Guid gameId)
        {
            var game = _database.Games.FirstOrDefault(x => x.Id == gameId);

            if (game != null)
            {
                game.ResetGame();
            }
            
            return game;
        }
    }
}