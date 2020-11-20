using System;
using System.Collections.Generic;
using System.Linq;
using ZoomersClient.Shared.Models;
using ZoomersClient.Shared.Models.Enums;

namespace ZoomersClient.Shared.Services
{
    public class GameService
    {        
        public List<Game> Games { get; set; }

        public GameService()
        {
            Games = new List<Game>();
        }

        public Game FindGame(Guid id)
        {
            var game = Games.FirstOrDefault(x => x.Id == id);

            foreach(var player in game.Players)
            {
                Console.WriteLine(player.Username + " still found!");
            }

            return game; 
        }

        public Game FindGame(List<PartyIcon> party)
        {
            // Console.WriteLine($"{party.Count()} items found. Checking {Games.Count()}");

            var game = Games.FirstOrDefault(x => x.Party[0] == party[0] && x.Party[1] == party[1] && x.Party[2] == party[2]);

            return game; 
        }

        public void CreateGame(Game game)
        {
            Games.Add(game);
        }

        public void JoinGame(Guid id, Player player)
        {
            var game = Games.FirstOrDefault(x => x.Id == id);

            if (game != null)
            {
                game.Players.Add(player);
                Console.WriteLine(player.Username + " just Add()");
            }
        }

        public void UpdateConnectionId(Guid id, string connectionId)
        {
            var game = Games.FirstOrDefault(x => x.Id == id);

            if (game != null)
            {
                game.ConnectionId = connectionId;
            }
        }

        public void EndGame(Guid id)
        {
            var game = Games.FirstOrDefault(x => x.Id == id);

            if (game != null)
            {
                game.EndGame();
            }
        }
    }
}