using ZoomersClient.Shared.Models.Enums;

namespace ZoomersClient.Shared.Models.DTOs
{
    public class ReactionDto 
    {
        public AnsweredQuestion Answer { get; set; }

        public AnswerReaction Reaction { get; set; }

        public ReactionDto(AnsweredQuestion answer, AnswerReaction reaction)
        {
            Answer = answer;
            Reaction = reaction;
        }
    }
}
