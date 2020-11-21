using System;
using ZoomersClient.Shared.Models.Enums;

namespace ZoomersClient.Shared.Models
{
    public class Player
    {
        public string ConnId { get; set; }
        public string Username { get; set; }
        public PlayerIcon Icon { get; set; }
        public string BackgroundColor { get; set;}
    }
}