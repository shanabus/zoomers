using System;
using System.Collections.Generic;
using ZoomersClient.Shared.Models.Enums;

namespace ZoomersClient.Shared.Models
{
    public class Game
    {
        public Guid Id { get; set;}

        public string Name { get; set; }

        public List<PartyIcon> Party { get; set; }

        public List<Player> Players { get; set;}

        public Game(string name)
        {
            Id = Guid.NewGuid();

            Name = name;

            Players = new List<Player>();

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

        static Random _R = new Random();
    
        static T RandomEnumValue<T> ()
        {
            var v = Enum.GetValues (typeof (T));
            return (T) v.GetValue (_R.Next(1,v.Length));
        }
    }
}