using System;
using ZoomersClient.Shared.Models.Enums;

namespace ZoomersClient.Shared.Models.DTOs
{
    public class PlayerDto
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public string ConnectionId { get; set; }
        public string Username { get; set; }
        public PlayerIcon Icon { get; set; }
        public string BackgroundColor { get; set;}
        public string Sound { get; set; }
        public bool OnDeck { get; set; }
        
        public int Score { get; set; }
        public int LoveScore { get; set; }
        public int HateScore { get; set; }

        public int LoveReactions { get; set; }
        public int HateReactions { get; set; }

        public int CorrectGuesses { get; set; }
    }
}