using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using ZoomersClient.Shared.Exceptions;
using ZoomersClient.Shared.Models;
using ZoomersClient.Shared.Models.Enums;

namespace ZoomersClient.Shared.Services
{
    public class GameService
    {        
        public List<Game> Games { get; set; }
        private ILogger<GameService> _logger { get; set; }

        public GameService(ILogger<GameService> logger)
        {
            _logger = logger;
            
            Games = new List<Game>();            
            
            SeedDefaultGame();
        }

        public Game SeedDefaultGame()
        {
            Games = new List<Game>();
            // https://postimg.cc/gallery/hxv1nzD/4752bea0
            var defaultGame = new Game("SWB Game", "en-GB", 3);
            defaultGame.Id = Guid.Parse("5b05a3a6-7665-47dd-b515-03372211a95e");
            Games.Add(defaultGame);

            return defaultGame;
        }

        public Game FindGame(Guid id)
        {
            var game = Games.FirstOrDefault(x => x.Id == id);

            return game; 
        }

        public Game FindGame(List<PartyIcon> party)
        {
            var game = Games.FirstOrDefault(x => x.Party[0] == party[0] && x.Party[1] == party[1] && x.Party[2] == party[2]);

            return game; 
        }

        public void CreateGame(Game game)
        {
            Games.Add(game);
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

        public Game AddQuestion(Guid id, WordPlayQuestion q)
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
    }
}