using System;
using System.Collections.Generic;
using System.Linq;
using ZoomersClient.Shared.Models.Enums;

namespace ZoomersClient.Shared.Models.DTOs
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public string Voice { get; set; }
        public string GameType{ get; set; }
        public int MinimumNumberOfPlayers { get; set; }
        public int MaximumNumberOfPlayers { get; set; }
        
        public string Party { get; set; }
        public int Rounds { get; set; }
        public int CurrentRound { get; set; }        
        public GameState State { get; set; }

        public List<PlayerDto> Players { get; set; }

        public List<GameQuestionDto> Questions { get; set; }
        public List<AnsweredQuestionDto> AnsweredQuestions { get; set; }
        public List<AudienceScoreDto> AudienceScore { get; set; }

        public PlayerDto CurrentPlayer => Players.FirstOrDefault(x => x.OnDeck);

        public GameDto()
        {
            Players = new List<PlayerDto>();
            Questions = new List<GameQuestionDto>();
        }

        public bool HasEnoughPlayers()
        {
            return Players.Count > MinimumNumberOfPlayers;
        }   

        public bool AskedEnoughQuestionsForRound()
        {
            return Questions.Any() && Questions.Count % Players.Count == 0;
        }

        public string[] GameAndAllPlayerConnections()
        {
            var conns = Players.Select(x => x.ConnectionId).ToList();
            conns.Add(ConnectionId);

            return conns.ToArray();
        }
        
        public string[] GameAndCurrentPlayerConnections()
        {
            var conns = new string[] { CurrentPlayer.ConnectionId, ConnectionId };
            return conns;
        }

        public string[] GameAndPlayerConnections(PlayerDto player)
        {
            var conns = new string[] { player.ConnectionId, ConnectionId };
            return conns;
        }

        public GameDto ShuffleAnswers()
        {
            var r = new Random();
            AnsweredQuestions.OrderBy(x => r.Next());
            
            return this;
        }

        public List<AnsweredQuestionDto> CorrectAnswers()
        {
            var answeredQuestions = new List<AnsweredQuestionDto>();

            foreach(var answer in AnsweredQuestions)
            {
                if (answer.Answer == answer.CurrentPlayerAnswer)
                {
                    answeredQuestions.Add(answer);
                }
            }
            
            return answeredQuestions;
        }

        public List<AnsweredQuestionDto> CurrentAnswers()
        {
            var lastQuestionAsked = Questions.LastOrDefault();

            return AnsweredQuestions.Where(x => lastQuestionAsked != null && x.Question.Id == lastQuestionAsked.Id).ToList();
        }
    }
}