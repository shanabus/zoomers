
using Microsoft.Extensions.Configuration;

namespace ZoomersClient.Shared.Models
{
    public class GameConfiguration
    {
        public string HostUrl { get; }

        public GameConfiguration(IConfiguration configuration)
        {
            HostUrl = configuration["Game:HostUrl"];
        }
    }
}