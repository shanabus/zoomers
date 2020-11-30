using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ZoomersClient.Server.Hubs
{
    public class DrawHub : Hub
    {
        private ILogger<DrawHub> _logger { get; set;}

        public DrawHub(ILogger<DrawHub> logger)
        {
            _logger = logger;
        }

        public async Task DrawImage(double prev_x, double prev_y, double x, double y, string clr){
            await Clients.All.SendAsync("UpdateImage", prev_x, prev_y, x, y, clr);
        }
    }
}