using System;
using System.Collections.Generic;
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
    public class GameHub : Hub
    {
        // for background service cross-hub comms, check out https://docs.microsoft.com/en-us/aspnet/core/signalr/background-services?view=aspnetcore-5.0

        private ILogger<GameService> _logger { get; set;}
        public List<string> Players { get; set; }
        public GameService _gameService { get; set;}        
        private Phrases _phrases { get; set; }

        public GameHub(ILogger<GameService> logger, GameService gameService, Phrases phrases)
        {
            Players = new List<string>();
            _gameService = gameService;
            _logger = logger;
            _phrases = phrases;
        }

        public async Task JoinTheGame(List<PartyIcon> partyIcons, string username, PlayerIcon icon, string color, string sound)
        {
            var game = _gameService.FindGame(partyIcons);
            if (game == null)
            {
                await Clients.Caller.SendAsync("PlayerJoinedError", "Could not find game");
            }
            else
            {                   
                var player = new Player() { 
                    Id = Guid.NewGuid(),
                    ConnectionId = Context.ConnectionId, 
                    Username = username,
                    Icon = icon,
                    BackgroundColor = color,
                    Sound = sound
                };
                _logger.LogInformation(player.Username + " connected as " + player.ConnectionId);
                
                var updatedGame = _gameService.JoinGame(game.Id, player);

                // let new player know
                await Clients.Caller.SendAsync("PlayerJoined", game, player);                

                var phrase = _phrases.GetRandomPlayerJoinedPhrase(username, updatedGame.Voice) as SpeechSynthesisUtterance;
                await Clients.All.SendAsync("PlayersUpdated", updatedGame, player, phrase);

                if (updatedGame.HasEnoughPlayers())
                {
                    // send to first player really
                    await Clients.All.SendAsync("ReadyToStartGame", updatedGame);
                }
            }
        }

        public async Task StartGame(Guid gameId)
        {
            // get game, start it (set date?)
            var game = _gameService.StartGame(gameId);

            // inform players game has started
            await Clients.All.SendAsync("GameStarted", gameId);
        }

        public void Subscribe(Guid id)
        {
            _gameService.UpdateConnectionId(id, Context.ConnectionId);
        }
    }
}