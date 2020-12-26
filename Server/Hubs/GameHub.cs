using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task JoinTheGame(List<string> partyIcons, PlayerDto player)
        {
            var game = await _gameService.FindGameAsync(partyIcons);
            if (game == null)
            {
                await Clients.Caller.SendAsync("PlayerJoinedError", "Could not find game");
            }
            else
            {   
                if (game.Players.Count < game.MaximumNumberOfPlayers)               
                {
                    // todo look for existing player
                    if (player.Id == default(Guid) || player.Id == Guid.Empty)
                    {
                        player.Id = Guid.NewGuid();
                    }

                    player.GameId = game.Id;
                    player.ConnectionId = Context.ConnectionId;
                                        
                    var updatedGame = await _gameService.JoinGameAsync(game.Id, player);

                    // let new player know
                    await Clients.Caller.SendAsync("PlayerJoined", updatedGame, player);                

                    var phrase = _phrases.GetRandomPlayerJoinedPhrase(player.Username, updatedGame.Voice) as SpeechSynthesisUtterance;
                    
                    // let everyone know                    
                    await Clients.Clients(GameAndPlayersConnections(updatedGame)).SendAsync("PlayersUpdated", updatedGame, player, phrase);

                    if (updatedGame.HasEnoughPlayers())
                    {
                        // send to first player really
                        await Clients.All.SendAsync("ReadyToStartGame", updatedGame);
                    }
                } 
                else
                {
                    // let new player know
                    await Clients.Caller.SendAsync("LobbyIsFull", player.Username);

                    // let the lobby know
                    // let new player know
                    await Clients.All.SendAsync("TooManyPlayersWarning", game.MaximumNumberOfPlayers, player.Username);
                }
            }
        }

        public async Task StartGame(Guid gameId)
        {
            // Console.WriteLine("StartGame");
            // get game, start it (set date?)
            var game = await _gameService.StartGameAsync(gameId);

            // inform players game has started
            await Clients.Clients(GameAndPlayersConnections(game)).SendAsync("GameStarted", gameId);
        }

        public async Task SubscribeAsync(Guid gameId)
        {
            await _gameService.UpdateConnectionIdAsync(gameId, Context.ConnectionId);
        }

        private List<string> GameAndPlayersConnections(GameDto game)
        {
            var clients = game.Players.Select(x => x.ConnectionId).ToList();
            clients.Add(game.ConnectionId);

            Console.WriteLine("Starting Game for " + clients.Count + " connections");

            return clients;
        }

        public override async Task OnDisconnectedAsync(Exception exception)  
        {  
            Console.Write("Disconnected: " + Context.ConnectionId);

            Guid gameId = await _gameService.RemoveDisconnectedPlayer(Context.ConnectionId);
                        
            if (gameId != Guid.Empty)
            {
                Console.WriteLine("Found a game client was connected to");
                var game = await _gameService.FindGameAsync(gameId);
                
                await Clients.Clients(GameAndPlayersConnections(game)).SendAsync("PlayersRemoved", game);                               
            }
            
            await base.OnDisconnectedAsync(exception);              
        }  
    }
}