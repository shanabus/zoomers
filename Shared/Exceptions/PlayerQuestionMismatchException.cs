using System;

namespace ZoomersClient.Shared.Exceptions
{
    public class PlayerQuestionMismatchException : Exception
    {
        public PlayerQuestionMismatchException(string message) : base(message)
        {
            
        }
    }
}