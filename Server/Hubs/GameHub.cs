using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ZoomersClient.Shared.Models;
using ZoomersClient.Shared.Models.Enums;
using ZoomersClient.Shared.Services;

namespace ZoomersClient.Server.Hubs
{
    public class GameHub : Hub
    {
        // for background service cross-hub comms, check out https://docs.microsoft.com/en-us/aspnet/core/signalr/background-services?view=aspnetcore-5.0

        public List<string> Players { get; set; }
        public GameService _gameService { get; set;}        

        public GameHub(GameService gameService)
        {
            Players = new List<string>();
            _gameService = gameService;
        }

        public async Task JoinGame(string user)
        {
            //if (!Players.Contains(user)){
                Players.Add(user);
            //}

            await Clients.All.SendAsync("PlayerJoined", Players);
        }

        public async Task JoinTheGame(List<PartyIcon> partyIcons, string username)
        {
            var game = _gameService.FindGame(partyIcons);
            if (game == null)
            {
                Console.WriteLine("No game found at Hub, send error");
                await Clients.Caller.SendAsync("PlayerJoinedError", "Could not find game");
            }
            else
            {                
                Console.WriteLine($"Joined game {username}");
                await Clients.Caller.SendAsync("PlayerJoined", game);
                
                var player = new Player() { ConnId = Context.ConnectionId, Username = username };
                _gameService.JoinGame(game.Id, player);   
                Console.WriteLine($"Found game {game.ConnectionId} to update with {game.Players.Count} players");

                await Clients.All.SendAsync("PlayersUpdated", game);
            }
        }

        public void Subscribe(Guid id)
        {
            Console.WriteLine("Joined sub"); 
            _gameService.UpdateConnectionId(id, Context.ConnectionId);
        }
    }
}