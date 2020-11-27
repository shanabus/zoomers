using System;
using System.Collections.Generic;
using System.Linq;
using ZoomersClient.Shared.Exceptions;
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
        public Player CurrentPlayer { get; set; }
        public List<AnsweredQuestion> AnsweredQuestions { get; set; }
        public List<QuestionBase> Questions { get; set; }

        public Game()
        {
            Party = new List<PartyIcon>();
            Players = new List<Player>();
            AnsweredQuestions = new List<AnsweredQuestion>();
            Questions = new List<QuestionBase>();
        }

        public Game(string name, string voice)
        {
            Id = Guid.NewGuid();
            Name = name;
            Voice = voice;

            Players = new List<Player>();
            AnsweredQuestions = new List<AnsweredQuestion>();
            Questions = new List<QuestionBase>();

            Party = new List<PartyIcon>() {
                RandomEnumValue<PartyIcon>(),
                RandomEnumValue<PartyIcon>(),
                RandomEnumValue<PartyIcon>()
            };
        }
        
        public Game StartGame()
        {
            State = GameState.Started;

            return this;
        }

        public void EndGame()
        {
            // do something that says its over
        }

        public void AnswerQuestion(Player player, int questionId, string answer)
        {
            AnsweredQuestions.Add(new AnsweredQuestion() {
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

        public void UpdatePlayerConnection(Guid playerId, string connectionId)
        {
            var player = Players.FirstOrDefault(x => x.Id == playerId);

            if (player != null)
            {
                player.ConnectionId = connectionId;
            }
        }

        public void RecordScore(int questionId, int score)
        {
            var q = Questions.FirstOrDefault(x => x.Id == questionId);

            if (q != null)
            {
                Players[Questions.Count - 1].Score += score;
            }
        }

        public Player GetNextPlayer()
        {
            try
            {
                // this assumes the question was asked first...
                CurrentPlayer = Players[(Questions.Count - 1) % Players.Count];
                return CurrentPlayer;
            }
            catch(Exception e) 
            {
                throw new PlayerQuestionMismatchException(e.Message);
            }
        }

        #region Game method chaining

        public Game ResetAnswers()
        {
            AnsweredQuestions = new List<AnsweredQuestion>();
            return this;
        }

        public Game ShufflePlayerOrder()
        {
            var rand = new Random();
            var randomList = Players.OrderBy(x => rand.Next()).ToList();

            return this;
        }

        #endregion
    }
}