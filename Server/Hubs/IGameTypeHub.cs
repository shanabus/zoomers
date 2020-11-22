using System.Threading.Tasks;

namespace ZoomersClient.Server.Hubs
{
    public interface IGameTypeHub
    {       

        Task AskQuestion();

        Task AnswerQuestion();

        Task NextQuestion();
    }
}