namespace ZoomersClient.Shared.Models
{
    public class QuestionBase
    {
        public int Id { get; set; }
        
        public string Question { get; set; }

        public string ImageUrl { get; set; }

        public QuestionBase()
        {
            
        }
    }
}