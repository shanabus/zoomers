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

        public string CategoriesString { get; set; }        
        
        public string[] Categories { get {
            return CategoriesString.Split(",");
            }
        }  
    }
}