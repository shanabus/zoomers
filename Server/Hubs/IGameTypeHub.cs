using System;
using System.Threading.Tasks;

namespace ZoomersClient.Server.Hubs
{
    public interface IGameTypeHub
    {       

        Task AskQuestion(Guid gameId);

        Task AnswerQuestion(Guid gameId, int questionId, Guid playerId, string answer);

        Task AnswersFinished(Guid gameId, string username);

        Task QuestionFinished(Guid gameId, int questionId, int score);
        
        Task UpdateConnectionId(Guid gameId, Guid playerId);
    }
}