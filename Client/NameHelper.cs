using System;

namespace ZoomersClient.Client
{
    public static class NameHelper
    {
        public static string[] FirstNames => new string[] {
            "Abby",
            "Ace",
            "Anna",
            "Amber",
            "Bam Bam",
            "Big",
            "Borf",
            "Captain",
            "Carol",
            "Charles",
            "Dax",
            "Derek",
            "Dante",
            "Emperor",
            "Erin",
            "Ernie",
            "Esteemed",
            "Famous",
            "Frank",
            "Gilbert",
            "Grand",
            "Gwen",
            "Haley",
            "Hank",
            "Harry",
            "Ian",
            "Jack",
            "Jennifer",
            "Karen",
            "Kimmy",
            "Lady",
            "Larry",
            "Lisa",
            "Little",
            "Longlasting",
            "Lonnie",
            "Lonely",
            "Lovely",
            "Lindsey",
            "Mark",
            "Mike",
            "Miss",
            "Mr.",
            "Nash",
            "Nedra",
            "Paulette",
            "Phil",
            "Sam",
            "Sharky",
            "Sharron",
            "Sir",
            "Suzy",
            "Tegan",
            "Todd",
            "Tracy",
            "Yeet",
            "Wally",
            "Wiz",
            "Zane"
        };
        
        public static string[] LastNames => new string[] {
            "in Charge",
            "in Deep",
            "in Love",
            "of Darkness",
            "of Nottingham",
            "of Yorkshire",
            "on the Run",
            "on Fire",
            "on Top",
            "the Ape",
            "the Fearsome",
            "the Fierce",
            "the Great",
            "the King",
            "the Lazy",
            "the Queen",
            "the Ripper",
            "the Slow",
            "the Wicked",
            "the Wise",
            "Baskin",
            "Blue",
            "Brown",
            "Dog",
            "Diggies",
            "Finklestein",
            "Frampton",
            "Green",
            "Howler",
            "Johnson",
            "McDonald",
            "McLovin",
            "Monkey",
            "Mountains",
            "Nyholt",
            "O'Malley",
            "Parrot",
            "Peebles",            
            "Pink",
            "Problems",
            "Samson",
            "Sievers",
            "Slime",
            "Smith",
            "Sweet",
            "Tavish",
            "Tiger",
            "Willis",
            "Yolo",
            "Zoomers",
            ""
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