using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ZoomersClient.Shared.Data;
using ZoomersClient.Shared.Exceptions;
using ZoomersClient.Shared.Models;
using ZoomersClient.Shared.Models.DTOs;
using ZoomersClient.Shared.Models.Enums;

namespace ZoomersClient.Shared.Services
{
    public class GameService
    {        
        private ILogger<GameService> _logger { get; set; }
        private ApplicationDBContext _database { get; set; }
        private readonly IMapper _mapper;

        public GameService(ILogger<GameService> logger, ApplicationDBContext database, IMapper mapper)
        {
            _logger = logger;
            _database = database;
            _mapper = mapper;
        }

        public List<GameDto> AllGames()
        {
            var games = _database.Games.ToList();

            return _mapper.Map<List<GameDto>>(games);
        }

        public async Task<GameDto> FindGameAsync(Guid id)
        {
            var game = await _database.Games.Include(x => x.Players).FirstOrDefaultAsync(x => x.Id == id);
            
            return _mapper.Map<GameDto>(game); 
        }

        public async Task SaveGameAsync(Game game)
        {
            _logger.LogInformation("Saving game");

            _database.Entry(game).State = EntityState.Modified;
            await _database.SaveChangesAsync();
        }

        public async Task<GameDto> AnswerQuestionAsync(Guid id, PlayerDto player, int questionId, string answer)
        {
            var game = await _database.Games.FirstOrDefaultAsync(x => x.Id == id);
    
            var p = game.Players.ToList().FirstOrDefault(x => x.Id == player.Id);

            game.AnswerQuestion(p, questionId, answer);

            await SaveGameAsync(game);

            return _mapper.Map<GameDto>(game);
        }

        public async Task RecordGuessAsync(Guid id, int questionId, Guid playerId, int guess)
        {
            var game = await _database.Games.FirstOrDefaultAsync(x => x.Id == id);

            game.RecordGuess(questionId, playerId, guess);

            await _database.SaveChangesAsync();
        }

        public async Task<GameDto> FindGameAsync(List<string> party)
        {
            var game = await _database.Games.FirstOrDefaultAsync(x => x.Party == string.Join('|', party));

            return _mapper.Map<GameDto>(game); 
        }

        public async Task<GameDto> CreateGameAsync(Game game)
        {
            _database.Games.Add(game);

            await _database.SaveChangesAsync();

            return _mapper.Map<GameDto>(game);
        }

        public async Task<GameDto> JoinGameAsync(Guid id, PlayerDto player)
        {
            var game = await _database.Games.Include(x => x.Players).FirstOrDefaultAsync(x => x.Id == id);

            if (game != null)
            {
                var existingPlayer = game.Players?.FirstOrDefault(x => x.Id == player.Id);
                if (existingPlayer != null)
                {
                    _logger.LogInformation("Found an existing player with same Id. Update connection?");                    
                }
                else                
                {
                    _logger.LogInformation("Adding player");
                    
                    var newPlayer = _mapper.Map<Player>(player);

                    _database.Players.Add(newPlayer);
                                                            
                    await SaveGameAsync(game);

                    //_logger.LogInformation(Extensions.Dump(game));
                }
                
            }

            var mapped = _mapper.Map<GameDto>(game);

            //_logger.LogInformation(Extensions.Dump(mapped));

            return mapped;
        }

        public async Task<GameDto> RecordScoresAsync(Guid id, int questionId, int score)
        {
            var game = await _database.Games.FirstOrDefaultAsync(x => x.Id == id);

            game.RecordScore(questionId, score).RecordGuesses(questionId);

            await SaveGameAsync(game);

            return _mapper.Map<GameDto>(game);
        }

        public async Task<PlayerDto> UpdatePlayerConnection(Guid id, Guid playerId, string connectionId)
        {
            var game = await _database.Games.FirstOrDefaultAsync(x => x.Id == id);
            
            var player = game.UpdatePlayerConnection(playerId, connectionId);

            await SaveGameAsync(game);

            return _mapper.Map<PlayerDto>(player);
        }

        public async Task<GameDto> UpdateGameConnection(Guid id, string connectionId)
        {
            var game = await _database.Games.FirstOrDefaultAsync(x => x.Id == id);
            
            game.UpdateGameConnection(connectionId);

            await SaveGameAsync(game);

            return _mapper.Map<GameDto>(game);
        }

        public async Task<GameDto> AddQuestionAsync(Guid id, QuestionBase q)
        {
            var game = await _database.Games
                .Include(x => x.Players)
                .Include(x => x.Questions)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (game != null) 
            {   
                game.ResetAnswers().Questions.Add(q as QuestionBase);

                game.GetNextPlayer();

                await SaveGameAsync(game);
            }

            return _mapper.Map<GameDto>(game);
        }

        public async Task DeleteAsync(Guid id)
        {
            var game = await _database.Games.FirstOrDefaultAsync(a=> a.Id == id);

            _database.Remove(game);

            await _database.SaveChangesAsync();
        }

        public async Task UpdateConnectionIdAsync(Guid gameId, string connectionId)
        {
            var game = await _database.Games.FirstOrDefaultAsync(x => x.Id == gameId);

            if (game != null)
            {
                game.ConnectionId = connectionId;
            }

            await SaveGameAsync(game);
        }

        public async Task<PlayerDto> GetNextPlayerAsync(Guid gameId)
        {
            var game = await _database.Games.FirstOrDefaultAsync(x => x.Id == gameId);

            if (game != null)
            {
                return _mapper.Map<PlayerDto>(game.GetNextPlayer());
            }
            
            throw new GameNotFoundException();
        }

        public async Task<GameDto> EndGameAsync(Guid gameId)
        {
            var game = await _database.Games.FirstOrDefaultAsync(x => x.Id == gameId);

            if (game != null)
            {
                return _mapper.Map<GameDto>(game.EndGame());
            }
            
            return _mapper.Map<GameDto>(game);
        }

        public async Task<GameDto> StartGameAsync(Guid gameId)
        {
            var game = await _database.Games.FirstOrDefaultAsync(x => x.Id == gameId);

            if (game != null)
            {
                game.StartGame();
            }

            await SaveGameAsync(game);

            return _mapper.Map<GameDto>(game);
        }

        public async Task<GameDto> StartNextRoundAsync(Guid gameId)
        {
            var game = await _database.Games.FirstOrDefaultAsync(x => x.Id == gameId);

            if (game != null)
            {
                game.NextRound();
            }
            
            return _mapper.Map<GameDto>(game);
        }

        public async Task<GameDto> AddAudienceReactionAsync(Guid gameId, Player fromPlayer, Player toPlayer, AnswerReaction reaction)
        {
            var game = await _database.Games.FirstOrDefaultAsync(x => x.Id == gameId);

            if (game != null)
            {
                game.AddReaction(fromPlayer, toPlayer, reaction);
            }
            
            return _mapper.Map<GameDto>(game);
        }

        public async Task<GameDto> ResetGameAsync(Guid gameId)
        {
            var game = await _database.Games.FirstOrDefaultAsync(x => x.Id == gameId);

            if (game != null)
            {
                game.ResetGame();
            }
            
            return _mapper.Map<GameDto>(game);
        }
    }    
}