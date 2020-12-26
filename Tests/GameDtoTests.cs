using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZoomersClient.Shared.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class GameDtoTests
    {
        [TestMethod]
        public void CanGetCurrentPlayer()
        {
            var gameDto = new GameDto();
            
            gameDto.Players = new List<PlayerDto>();

            gameDto.Players.Add(new PlayerDto() { Username = "Amos", OnDeck = true });
            gameDto.Players.Add(new PlayerDto() { Username = "Beth" });
            gameDto.Players.Add(new PlayerDto() { Username = "Carl" });

            Assert.IsTrue(gameDto.CurrentPlayer != null);
        }
    }
}
