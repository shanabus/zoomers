using System;
using System.Collections.Generic;
using System.Linq;
using ZoomersClient.Shared.Models.Enums;

namespace ZoomersClient.Shared.Models
{
    public class Game
    {
        public Guid Id { get; set;}

        public string ConnectionId { get; set; }

        public string Name { get; set; }
        public string Voice { get; set; }
        public string GameType = "wordplay";
        public int MinimumNumberOfPlayers => 2;
        
        public GameState State { get; set; }

        public List<PartyIcon> Party { get; set; }

        public List<Player> Players { get; set;}

        public List<AnsweredQuestions> AnsweredQuestions { get; set; }
        public List<QuestionBase> Questions { get; set; }

        public Game()
        {
            Party = new List<PartyIcon>();
            Players = new List<Player>();
            AnsweredQuestions = new List<AnsweredQuestions>();
            Questions = new List<QuestionBase>();
        }

        public Game(string name, string voice)
        {
            Id = Guid.NewGuid();
            Name = name;
            Voice = voice;

            Players = new List<Player>();
            AnsweredQuestions = new List<AnsweredQuestions>();
            Questions = new List<QuestionBase>();

            Party = new List<PartyIcon>() {
                RandomEnumValue<PartyIcon>(),
                RandomEnumValue<PartyIcon>(),
                RandomEnumValue<PartyIcon>()
            };
        }

        public void EndGame()
        {
            // do something that says its over
        }

        public void AnswerQuestion(Player player, int questionId, string answer)
        {
            AnsweredQuestions.Add(new AnsweredQuestions() {
                Player = player,
                Question = Questions.FirstOrDefault(x => x.Id == questionId),
                Answer = answer
            });
        }

        static Random _R = new Random();
    
        static T RandomEnumValue<T> ()
        {
            var v = Enum.GetValues (typeof (T));
            return (T) v.GetValue (_R.Next(1,v.Length));
        }
    }
}