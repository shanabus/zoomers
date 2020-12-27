using System.ComponentModel.DataAnnotations;

namespace ZoomersClient.Shared.Models.DTOs
{
    public class EditQuestionDto
    {
        [Required]        
        public int Id { get; set; }

        [Required]
        public string Question { get; set; }

        public string ImageUrl { get; set; }

        public string Categories { get; set; }
    }
}