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
        public const int MinimumNumberOfPlayers = 2;
        public const int Rounds = 2;
        public int CurrentRound { get; set; }
        
        public GameState State { get; set; }
        public List<PartyIcon> Party { get; set; }
        public List<Player> Players { get; set;}        
        public Player CurrentPlayer { get; set; }
        public List<AnsweredQuestion> AnsweredQuestions { get; set; }
        public List<QuestionBase> Questions { get; set; }
        public List<AudienceScore> AudienceScore { get; set; }

        public Game()
        {
            Party = new List<PartyIcon>();
            Players = new List<Player>();
            AnsweredQuestions = new List<AnsweredQuestion>();
            Questions = new List<QuestionBase>();
            AudienceScore = new List<AudienceScore>();
            CurrentRound = 1;
        }

        public Game(string name, string voice)
        {
            Id = Guid.NewGuid();
            Name = name;
            Voice = voice;

            Players = new List<Player>();
            AnsweredQuestions = new List<AnsweredQuestion>();
            Questions = new List<QuestionBase>();
            AudienceScore = new List<AudienceScore>();

            Party = new List<PartyIcon>() {
                RandomEnumValue<PartyIcon>(),
                RandomEnumValue<PartyIcon>(),
                RandomEnumValue<PartyIcon>()
            };

            CurrentRound = 1;
        }
        
        public Game StartGame()
        {
            State = GameState.Started;

            return this;
        }

        public Game EndGame()
        {
            State = GameState.Ended;
            Players = Players.OrderByDescending(x => x.Score).ToList();
            return this;
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

        public Game RecordScore(int questionId, int score)
        {
            var q = Questions.FirstOrDefault(x => x.Id == questionId);

            if (q != null)
            {
                Players[Questions.Count - 1].Score += score;
            }

            return this;
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

        internal Game AddLove(Player fromPlayer, Player toPlayer)
        {
            var existingScore = AudienceScore.FirstOrDefault(x => x.Round == CurrentRound && x.FromPlayerId == fromPlayer.Id && x.ToPlayerId == toPlayer.Id);

            if (existingScore == null)
            {
                AudienceScore.Add(new Models.AudienceScore {
                    Round = CurrentRound,
                    FromPlayerId = fromPlayer.Id,
                    ToPlayerId = toPlayer.Id,
                    Score = 1
                });
            }
            else
            {
                existingScore.Score = existingScore.Score + 1;
            }
            return this;
        }

        public Game ShuffleAnswers()
        {
            var r = new Random();
            AnsweredQuestions.OrderBy(x => r.Next());
            
            return this;
        }

        public bool HasEnoughPlayers()
        {
            return Players.Count() >= MinimumNumberOfPlayers;
        }

        #region Game method chaining

        public Game ResetAnswers()
        {
            AnsweredQuestions = new List<AnsweredQuestion>();
            return this;
        }

        public Game ResetQuestions()
        {
            Questions = new List<QuestionBase>();
            return this;
        }

        public Game ShufflePlayerOrder()
        {
            var rand = new Random();
            Players = Players.OrderBy(x => rand.Next()).ToList();

            return this;
        }

        public Game NextRound()
        {   
            if (CurrentRound < Rounds)         
            {
                CurrentRound++;
                return this.ResetAnswers().ResetQuestions().ShufflePlayerOrder();
            }
            
            return this.EndGame();            
        }

        #endregion
    }
}