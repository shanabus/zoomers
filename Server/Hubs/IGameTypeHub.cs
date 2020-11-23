using System;
using System.Threading.Tasks;

namespace ZoomersClient.Server.Hubs
{
    public interface IGameTypeHub
    {       

        Task AskQuestion(Guid id);

        Task AnswerQuestion(Guid id, int questionId, string answer);

        Task NextQuestion(Guid id);
    }
}