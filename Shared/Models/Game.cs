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
        public int Rounds { get; set; }
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

        public Game(string name, string voice, int rounds)
        {
            Id = Guid.NewGuid();
            Name = name;
            Voice = voice;
            Rounds = rounds;

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
            
            return this.ShufflePlayerOrder();
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

        public Game UpdateGameConnection(Guid gameId, string connectionId)
        {
            ConnectionId = connectionId;

            return this;
        }

        public Player UpdatePlayerConnection(Guid playerId, string connectionId)
        {
            var player = Players.FirstOrDefault(x => x.Id == playerId);

            if (player != null)
            {
                player.ConnectionId = connectionId;
            }

            return player;
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

        internal Game AddReaction(Player fromPlayer, Player toPlayer, AnswerReaction reaction)
        {
            // var existingScore = AudienceScore.FirstOrDefault(x => x.Round == CurrentRound && x.FromPlayerId == fromPlayer.Id && x.ToPlayerId == toPlayer.Id);

            // if (existingScore == null)
            // {
            //     AudienceScore.Add(new Models.AudienceScore {
            //         Round = CurrentRound,
            //         FromPlayerId = fromPlayer.Id,
            //         ToPlayerId = toPlayer.Id,
            //         Score = reaction == AnswerReaction.Love? 1 : 0
            //     });
            // }
            // else
            // {
            //     existingScore.Score = existingScore.Score + 1;
            // }
            var player = Players.FirstOrDefault(x => x.Id == toPlayer.Id);
            
            if (player != null)
            {
                if (reaction == AnswerReaction.Hate)
                {
                    player.HateScore++;
                }

                if (reaction == AnswerReaction.Love)
                {
                    player.LoveScore++;
                }
            }

            var playerReacting = Players.FirstOrDefault(x => x.Id == toPlayer.Id);
            
            if (playerReacting != null)
            {
                if (reaction == AnswerReaction.Hate)
                {
                    playerReacting.HateReactions++;
                }

                if (reaction == AnswerReaction.Love)
                {
                    playerReacting.LoveReactions++;
                }
            }

            return this;
        }

        public Game ShuffleAnswers()
        {
            var r = new Random();
            AnsweredQuestions.OrderBy(x => r.Next());
            
            return this;
        }
        
        public string[] GameAndCurrentPlayerConnections()
        {
            var conns = new string[] { CurrentPlayer.ConnectionId, ConnectionId };
            return conns;
        }

        public string[] GameAndPlayerConnections(Player player)
        {
            var conns = new string[] { player.ConnectionId, ConnectionId };
            return conns;
        }

        public string[] GameAndAllPlayerConnections()
        {
            var conns = Players.Select(x => x.ConnectionId).ToList();
            conns.Add(ConnectionId);

            return conns.ToArray();
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