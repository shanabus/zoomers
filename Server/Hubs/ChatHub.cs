using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ZoomersClient.Server.Services;

namespace ZoomersClient.Server.Hubs
{
    public class ChatHub : Hub
    {
        public WordPlay _game;

        public ChatHub(WordPlay game)
        {
            _game = game;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task AskQuestion(string user, string category)
        {
            var question = _game.GetRandomQuestion(category);

            if (question != null)
            {
                await Clients.All.SendAsync("NewQuestion", user, question.Question, _game.GetRemainingCategories());
            }
            else {
                await Clients.Caller.SendAsync("NewQuestion", user, "Sorry, no more questions :(", new List<string>());
            }
        }

        public async Task GetCategories()
        {
            await Clients.All.SendAsync("SetCategories", _game.GetRemainingCategories());
        }
    }
}