using System;

namespace FirebaseSharp.Firebase.Models
{
    public class ObserveEventArgs : EventArgs
    {
        public ObserveEventArgs(string result, string path)
        {
            LowData = result;
            Path = path;
        }

        public string LowData { get; private set; }
        public string Path { get; private set; }
    }
}
