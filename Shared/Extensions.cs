using ZoomersClient.Shared.Models.Enums;

namespace ZoomersClient.Shared
{
    public static class Extensions
    {
        public static string ToString(this AnswerReaction reaction)
        {
            switch (reaction)
            {
                case AnswerReaction.Hate:
                    return "💩";
                case AnswerReaction.Love:
                    return "💕";
                default:
                    return "";
            }
        }
    }
    
}