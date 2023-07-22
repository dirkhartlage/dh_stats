using System;

namespace dh_stats.Exceptions
{
    public class UnsupportedTypeException : Exception
    {
        public UnsupportedTypeException() : base()
        {
        }

        public UnsupportedTypeException(string message) : base(message)
        {
        }

        public UnsupportedTypeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}