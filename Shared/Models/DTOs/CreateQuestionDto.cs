using System.ComponentModel.DataAnnotations;

namespace ZoomersClient.Shared.Models.DTOs
{
    public class CreateQuestionDto
    {        
        [Required]
        public string Question { get; set; }

        public string ImageUrl { get; set; }

        public string Categories { get; set; }
    }
}