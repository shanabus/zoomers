namespace ZoomersClient.Shared.Models
{
    public class AnsweredQuestions {
        public Player Player { get; set; }

        public QuestionBase Question { get; set; }

        public string Answer { get; set; }
    }
}