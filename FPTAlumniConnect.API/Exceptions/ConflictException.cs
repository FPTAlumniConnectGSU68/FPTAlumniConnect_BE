using System;

namespace FPTAlumniConnect.API.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
