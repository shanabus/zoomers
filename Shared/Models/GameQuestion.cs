using System.ComponentModel.DataAnnotations.Schema;

namespace ZoomersClient.Shared.Models
{

    [Table("Questions")]
    public class GameQuestion
    {
        public int Id { get; set; }

        public int QuestionBaseId { get; set; }
        
        public string Question { get; set; }

        public string ImageUrl { get; set; }

        public GameQuestion()
        {
            
        }
        
        public string CategoriesString { get; set; }        
        
        public string[] Categories { get {
            return CategoriesString.Split(",");
            }
        } 
    }
}