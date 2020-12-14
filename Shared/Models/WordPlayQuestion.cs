namespace ZoomersClient.Shared.Models
{
    public class WordPlayQuestion : QuestionBase
    {
        public string CategoriesString { get; set; }        
        public string[] Categories { get {
            return CategoriesString.Split(",");
            }
        }  
    }
}