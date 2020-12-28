namespace ZoomersClient.Shared.Models.DTOs
{
    public class GameQuestionDto
    {
        public int Id { get; set; }
        
        public int QuestionBaseId { get; set; }
        
        public string Question { get; set; }

        public string ImageUrl { get; set; }

        public string[] Categories { get; set; }
    }
}