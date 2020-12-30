using System;

namespace ZoomersClient.Shared.Models.DTOs
{
    public class AnsweredQuestionDto
    {
        public int Id { get; set; }  

        public Guid GameId { get; set; }

        public PlayerDto Player { get; set; }

        public GameQuestionDto Question { get; set; }

        public int Round { get; set; }

        public string Answer { get; set; }

        public string CurrentPlayerAnswer { get; set; }

        public int? Guess { get; set; }

        public bool AnswerTimedOut { get; set; }
    }
}