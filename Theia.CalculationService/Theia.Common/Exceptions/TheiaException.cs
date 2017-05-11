using System;

namespace Theia.Common.Exceptions
{
    public class TheiaException : Exception
    {
        public TheiaException(string message) : base(message)
        {
        }
    }
}