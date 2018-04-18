using System;

namespace FirebaseSharp.FireBase.Error
{
    public class FirebaseException :Exception
    {
        public FirebaseException() : base()
        {

        }
        public FirebaseException(string message) : base(message)
        {

        }

        public FirebaseException(string message, Exception exception) : base(message, exception)
        {

        }
    }
}
