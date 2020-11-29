namespace ZoomersClient.Shared.Models.DTOs
{    
    public class CreateGameDto
    {
        public string Name { get; set; }

        public int Rounds { get; set; }
        
        public string Voice { get; set; }
    }
}