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

        public async Task DrawImage(double prev_x, double prev_y, double x, double y, string color, float size)
        {
            await Clients.AllExcept(Context.ConnectionId).SendAsync("UpdateImage", prev_x, prev_y, x, y, color, size);
        }

        public async Task UploadImage(string data) 
        {
            _logger.LogInformation("added image to hub");
            await Clients.AllExcept(Context.ConnectionId).SendAsync("AddImage", data);
        }
    }
}