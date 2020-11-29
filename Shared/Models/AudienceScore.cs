using System;

namespace ZoomersClient.Shared.Models
{
    public class AudienceScore
    {
        public int Round { get; set; }

        public Guid FromPlayerId { get; set; } 

        public Guid ToPlayerId { get; set; }

        public int Score { get; set; }
    }
}