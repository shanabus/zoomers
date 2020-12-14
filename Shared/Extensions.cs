using ZoomersClient.Shared.Models.Enums;

namespace ZoomersClient.Shared
{
    public static class Extensions
    {
        public static string ToEmoji(this AnswerReaction reaction)
        {
            switch (reaction)
            {
                case AnswerReaction.Hate:
                    return "💩";
                case AnswerReaction.Love:
                    return "💕";
                case AnswerReaction.Trophy:
                    return "🏆";
                default:
                    return "";
            }
        }
    }
    
}