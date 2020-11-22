using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ZoomersClient.Server.Services;

namespace ZoomersClient.Server.Hubs
{
    public class WordPlayHub : Hub, IGameTypeHub
    {
        private WordPlay _wordPlay { get; set; }
        private ILogger<WordPlayHub> _logger { get; set; }
        
        public WordPlayHub(ILogger<WordPlayHub> logger, WordPlay wordplay)
        {
            _logger = logger;
            _wordPlay = wordplay;
        }
        
        public Task AnswerQuestion()
        {
            throw new System.NotImplementedException();
        }

        public async Task AskQuestion()
        {
            var q = _wordPlay.GetRandomQuestion(null);
            _logger.LogDebug(q.Question);

            await Clients.Caller.SendAsync("QuestionReady", q as QuestionBase);
        }

        public Task NextQuestion()
        {
            throw new System.NotImplementedException();
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();


        }
    }
}