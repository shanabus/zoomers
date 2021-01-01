using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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
        public int MinimumNumberOfPlayers = 2;
        public int MaximumNumberOfPlayers = 8;
        public int Rounds { get; set; }
        public int CurrentRound { get; set; }
        
        public GameState State { get; set; }
        public string Party { get; set; }        
        public List<Player> Players { get; set; }
        public List<AnsweredQuestion> AnsweredQuestions { get; set; }
        public List<GameQuestion> Questions { get; set; }
        public List<AudienceScore> AudienceScore { get; set; }

        public Game()
        {            
        }

        public Game(string name, string voice, int rounds)
        {
            Id = Guid.NewGuid();
            // Console.WriteLine("Game ctor overload about to call init");
            Init();

            Name = name;
            Voice = voice;
            Rounds = rounds;
        }

        private void Init()
        {
            Players = new List<Player>();
            AnsweredQuestions = new List<AnsweredQuestion>();
            Questions = new List<GameQuestion>();
            AudienceScore = new List<AudienceScore>();
            
            State = GameState.Lobby;

            Party = $"{RandomEnumValue<PartyIcon>()}|{RandomEnumValue<PartyIcon>()}|{RandomEnumValue<PartyIcon>()}";

            CurrentRound = 1;
        }

        public Game RecordGuess(int questionId, Guid playerId, int guess)
        {
            var playerAnswer = AnsweredQuestions.FirstOrDefault(x => x.Question.Id == questionId && x.Player.Id == playerId);

            if (playerAnswer != null)
            {
                playerAnswer.Guess = guess;
            }
            else
            {
                Console.WriteLine("player answer not found to record guess!");
            }

            return this;
        }

        public Game AddQuestion(GameQuestion q)
        {
            if (Questions == null)
            {
                Questions = new List<GameQuestion>();
            }
            Questions.Add(q);

            return this;
        }

        public Game StartGame()
        {
            State = GameState.Started;
            
            return this.ShufflePlayerOrder();
        }

        public Game EndGame()
        {
            State = GameState.Ended;
            Players = Players.OrderByDescending(x => x.Score).ThenBy(x => x.LoveReactions).ToList();
            return this;
        }

        public void AnswerQuestion(Player player, int questionId, string answer)
        {
            AnsweredQuestions.Add(new AnsweredQuestion() {
                Player = player,
                Question = Questions.FirstOrDefault(x => x.Id == questionId),
                Answer = answer,
                Round = CurrentRound
            });
        }

        public List<AnsweredQuestion> CorrectAnswers()
        {
            var answeredQuestions = new List<AnsweredQuestion>();

            foreach(var answer in AnsweredQuestions)
            {
                if (answer.Answer == answer.CurrentPlayerAnswer)
                {
                    answeredQuestions.Add(answer);
                }
            }
            
            return answeredQuestions;
        }
        
        public Game ResetGame()
        {
            Init();

            return this;
        }

        static Random _R = new Random();    
        static T RandomEnumValue<T> ()
        {
            var v = Enum.GetValues (typeof (T));
            return (T) v.GetValue (_R.Next(1,v.Length));
        }

        public Game UpdateGameConnection(string connectionId)
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

        public Game RecordGuesses(int questionId)
        {
            // need to know correct questions!
            var answers = AnsweredQuestions.Where(x => x.Question.Id == questionId);
            var numberCorrect = CorrectAnswers().Count;

            foreach(var answer in answers) 
            {
                if (answer.Guess == numberCorrect)
                {
                    var player = Players.FirstOrDefault(x => x.Id == answer.Player.Id);
                    player.Score += 50;
                    player.CorrectGuesses++;
                }
            }
            return this;
        }

        public Game RecordScore(int questionId, int score)
        {
            var q = Questions.FirstOrDefault(x => x.Id == questionId);

            if (q != null)
            {
                Players.FirstOrDefault(x => x.OnDeck).Score += score;
            }

            return this;
        }

        public Game PickNextPlayer()
        {
            try
            {
                Players.ForEach(p => p.OnDeck = false);

                var mod = (Questions.Count - 1) % Players.Count;

                // this assumes the question was asked first...
                Players[mod].OnDeck = true;
                return this;
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.Message);
                throw new PlayerQuestionMismatchException(e.Message);
            }
        }

        public Game AddReaction(Guid fromPlayerId, Guid toPlayerId, AnswerReaction reaction)
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
            
            var player = Players.FirstOrDefault(x => x.Id == toPlayerId);
            
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

            var playerReacting = Players.FirstOrDefault(x => x.Id == fromPlayerId);
            
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
        
        public Game ShufflePlayerOrder()
        {
            var rand = new Random();
            Players = Players.OrderBy(x => rand.Next()).ToList();

            Players.ForEach(x => Console.WriteLine(x.Username));
            Console.WriteLine("^ Shuffled Players ^");
            return this;
        }

        public Game NextRound()
        {   
            if (CurrentRound < Rounds)         
            {
                CurrentRound += 1;
                State = GameState.RoundSummary;
                return this.ShufflePlayerOrder();
            }
            
            return this.EndGame();            
        }
    }
}