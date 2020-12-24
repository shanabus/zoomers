namespace ZoomersClient.Shared.Models.DTOs
{
    public class QuestionDto
    {
        public int Id { get; set; }
        
        public string Question { get; set; }

        public string ImageUrl { get; set; }

        public string[] Categories { get; set; }
    }
}