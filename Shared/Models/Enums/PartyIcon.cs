
using System;

namespace ZoomersClient.Shared.Models.Enums
{
    [Flags]
    public enum PartyIcon
    {
        None = 0,
        Girl = 1,
        Boy = 2,
        Grandma = 4,
        Grandpa = 8,
        Scientist = 16,
        Cat = 32,
        Dog = 64,
        Zombie = 128,
        Family = 256
    }
}