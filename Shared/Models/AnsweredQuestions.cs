using System;

namespace ZoomersClient.Shared.Models
{
    public class AnsweredQuestion {      
        
        public int Id { get; set; }  

        public Guid GameId { get; set; }

        public Player Player { get; set; }

        public QuestionBase Question { get; set; }

        public int Round { get; set; }

        public string Answer { get; set; }

        public string CurrentPlayerAnswer { get; set; }

        public int Guess { get; internal set; }
    }
}