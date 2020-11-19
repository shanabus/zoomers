using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ZoomersClient.Server.Services;

namespace ZoomersClient.Server.Hubs
{
    public class GameHub : Hub
    {
        public List<string> Players { get; set; }

        public GameHub()
        {
            Players = new List<string>();
        }

        public async Task JoinGame(string user)
        {
            //if (!Players.Contains(user)){
                Players.Add(user);
            //}

            await Clients.All.SendAsync("PlayerJoined", Players);
        }
    }
}