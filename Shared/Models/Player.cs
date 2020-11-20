namespace ZoomersClient.Shared.Models
{
    public class Player
    {
        public readonly string ConnId;
        public readonly string Username;

        public Player(string connId, string username)
        {
            ConnId = connId;
            Username = username;
        }
    }
}