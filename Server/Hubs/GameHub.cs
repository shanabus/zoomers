using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ZoomersClient.Server.Services;
using ZoomersClient.Shared.Models.Enums;
using ZoomersClient.Shared.Services;

namespace ZoomersClient.Server.Hubs
{
    public class GameHub : Hub
    {
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
            Console.WriteLine("here with a new party!");

            var game = _gameService.FindGame(partyIcons);
            if (game == null)
            {
                Console.WriteLine("No game found at Hub, send error");
                await Clients.Caller.SendAsync("PlayerJoinedError", "Could not find game");
            }
            else
            {
                Console.WriteLine($"Game found, calling PlayerJoined with game {game.Name}");
                await Clients.Caller.SendAsync("PlayerJoined", game);
            }
        }
    }
}