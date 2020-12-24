using System;

namespace ZoomersClient.Shared.Models.DTOs
{
    public class AudienceScoreDto
    {
        public int Id { get; set; }
        
        public Guid GameId { get; set; }

        public int Round { get; set; }

        public Guid FromPlayerId { get; set; } 

        public Guid ToPlayerId { get; set; }

        public int Score { get; set; }
    }
}