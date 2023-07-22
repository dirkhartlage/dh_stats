using System;

namespace dh_stats.Exceptions
{
    public class AlreadyInitializedException : Exception
    {
        public AlreadyInitializedException() : base()
        {
        }

        public AlreadyInitializedException(string message) : base(message)
        {
        }

        public AlreadyInitializedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}