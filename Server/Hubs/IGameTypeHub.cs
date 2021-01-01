using System;
using System.Threading.Tasks;
using ZoomersClient.Shared.Models.DTOs;

namespace ZoomersClient.Server.Hubs
{
    public interface IGameTypeHub
    {       

        Task AskQuestion(Guid gameId, string category);

        Task AnswerQuestion(Guid gameId, int questionId, Guid playerId, string answer);

        Task AnswersFinished(Guid gameId);

        Task QuestionFinished(Guid gameId, int questionId, int score);
        
        Task UpdatePlayerConnectionId(Guid gameId, Guid playerId);
        
        Task UpdateGameConnectionId(Guid gameId);
    }
}