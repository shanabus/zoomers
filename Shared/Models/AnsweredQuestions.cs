namespace ZoomersClient.Shared.Models
{
    public class AnsweredQuestion {
        public Player Player { get; set; }

        public QuestionBase Question { get; set; }

        public string Answer { get; set; }
        public int Guess { get; internal set; }
    }
}