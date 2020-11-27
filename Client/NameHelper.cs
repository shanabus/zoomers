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
            "Lonnie",
            "Phil",
            "Lisa",
            "Mr.",
            "Miss",
            "Sir",
            "Big",
            "Little",
            "Lady",
            "Emperor",
            "Grand",
            "Esteemed",
            "Longlasting",
            "Famous",
            "Lonely",
            "Lovely",
            "Larry",
            "Harry"
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
            "Problems",
            "Brown",
            "Green",
            "Pink",
            "Blue",
            "Slime",
            "the Ripper",
            "the Wicked",
            "the Fearsome",
            "",
        };

        public static string[] LobbyFirstNames => new string[] {
            "Cheese",
            "Haunted",
            "Home",
            "Chilling",
            "Laugh",
            "Games",
            "Learning",
            "Party",
            "Pillow",
            "Drinks",
            "Pain",
            "Questions",
            "Pandemic",
            "Crazy",
            "Feeling Rough",
            "Getting Slimey",
            "Mobs",
            "Sticky",
            "Open",
            "Primo"
        };

        public static string[] LobbyLastNames => new string[] {
            "with Friends",
            "Haven",
            "Factory",
            "a Lot",
            "Garage",
            "Fight",
            "School",
            "of Nowhere",
            "in Trouble",
            "for the Win",
            "of all Time",
            "Home",
            "Party",
            "First",
            "to the Max",
            "Stacks",
            "Vibes",
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

        public static string GetRandomLobbyName()
        {
            var r = new Random();
            
            var first = LobbyFirstNames[r.Next(LobbyFirstNames.Length)];
            var last = LobbyLastNames[r.Next(LobbyLastNames.Length)];

            var name = $"{first} {last}".Trim();

            return name;
        }
    }
}