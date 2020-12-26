using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZoomersClient.Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void CanGetCurrentPlayer()
        {
            var game = new Game();
            game.Questions = new List<QuestionBase>();
            game.Questions.Add(new QuestionBase());

            game.Players = new List<Player>();

            game.Players.Add(new Player() { Username = "Amos" });
            game.Players.Add(new Player() { Username = "Beth" });
            game.Players.Add(new Player() { Username = "Carl" });

            game = game.PickNextPlayer();

            Assert.IsTrue(game.Players.FirstOrDefault(x => x.OnDeck) != null);
            Assert.IsTrue(game.Players.FirstOrDefault(x => x.OnDeck).Username == "Amos");
        }
    }
}
