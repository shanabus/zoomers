using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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

        public GameHub(ILogger<GameService> logger, GameService gameService)
        {
            Players = new List<string>();
            _gameService = gameService;
            _logger = logger;
        }

        public async Task JoinGame(string user)
        {
            if (!Players.Contains(user)){
                Players.Add(user);
            }

            await Clients.All.SendAsync("PlayerJoined", Players);
        }

        public async Task JoinTheGame(List<PartyIcon> partyIcons, string username, PlayerIcon icon, string color)
        {
            var game = _gameService.FindGame(partyIcons);
            if (game == null)
            {
                await Clients.Caller.SendAsync("PlayerJoinedError", "Could not find game");
            }
            else
            {                
                await Clients.Caller.SendAsync("PlayerJoined", game);
                
                var player = new Player() { 
                    ConnId = Context.ConnectionId, 
                    Username = username,
                    Icon = icon,
                    BackgroundColor = color    
                };
                _logger.LogInformation(player.Username + " connected");

                var updatedGame = _gameService.JoinGame(game.Id, player);

                //await Clients.All.SendAsync("PlayersUpdated", updatedGame);
                await Clients.All.SendAsync("PlayersUpdated", updatedGame);
            }
        }

        public void Subscribe(Guid id)
        {
            _gameService.UpdateConnectionId(id, Context.ConnectionId);
        }
    }
}