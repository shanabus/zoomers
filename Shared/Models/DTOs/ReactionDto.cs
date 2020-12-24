using ZoomersClient.Shared.Models.Enums;

namespace ZoomersClient.Shared.Models.DTOs
{
    public class ReactionDto 
    {
        public AnsweredQuestionDto Answer { get; set; }

        public AnswerReaction Reaction { get; set; }

        public ReactionDto(AnsweredQuestionDto answer, AnswerReaction reaction)
        {
            Answer = answer;
            Reaction = reaction;
        }
    }
}
