using System;

namespace DungeonAPI.Exceptions
{
    public class NoRoomsToGenerateFromException : Exception
    {
        public NoRoomsToGenerateFromException() { }
        public NoRoomsToGenerateFromException(string message) : base(message) { }
        public NoRoomsToGenerateFromException(string message, Exception inner) : base(message, inner) { }
        protected NoRoomsToGenerateFromException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
