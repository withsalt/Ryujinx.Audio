using System;

namespace Ryujinx.HLE.Exceptions
{
    public class InternalServiceException : Exception
    {
        public InternalServiceException(string message) : base(message) { }
    }
}
