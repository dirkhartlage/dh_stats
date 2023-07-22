using System;

namespace StatsSystem.Exceptions
{
    public class NotInitializedException : Exception
    {
        public NotInitializedException() : base()
        {
        }

        public NotInitializedException(string message) : base(message)
        {
        }

        public NotInitializedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}