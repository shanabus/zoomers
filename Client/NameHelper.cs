using System;

namespace ZoomersClient.Client
{
    public static class NameHelper
    {
        public static string[] FirstNames => new string[] {
            "Sam",
            "Mark",
            "Abby",
            "Sharky",
            "Paulette",
            "Frank",
            "Suzy",
            "Karen",
            "Carol",
            "Lindsey",
            "Jennifer",
            "Ace",
            "Dax",
            "Tegan",
            "Wiz",
            "Zane",
            "Captain",
            "Sharon",
            "Hank",
            "Jack",
            "Charles",
            "Todd",
            "Erin",
            "Nash",
            "Anna",
            "Mike",
            "Tracy",
            "Nedra",
            "Kimmy",
            "Haley",
            "Phil",
            "Lisa",
            "Mr.",
            "Miss",
            "Sir",
            "Big",
            "Little",
            "Lady"
        };
        
        public static string[] LastNames => new string[] {
            "the Great",
            "the Wise",
            "McDonald",
            "Johnson",
            "Smith",
            "Baskin",
            "Frampton",
            "O'Malley",
            "of Yorkshire",
            "of Nottingham",
            "the King",
            "the Queen",
            "of Darkness",
            "in Love",
            "on the Run",
            "on Fire",
            "the Fierce",
            "Sievers",
            "Nyholt",
            "McLovin",
            "Tavish",
            "Mountain",
            "Samson",
            "Finklestein",
            "Zoomers",
            "Smith",
            "Dog",
            "Monkey",
            "Tiger",
            "in Charge",
            "",
        };

        public static string GetRandomName()
        {
            var r = new Random();
            
            var first = FirstNames[r.Next(FirstNames.Length)];
            var last = LastNames[r.Next(LastNames.Length)];

            var name = $"{first} {last}".Trim();

            return name;
        }
    }
}