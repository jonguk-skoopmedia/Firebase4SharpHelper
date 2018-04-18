using FirebaseSharp.FireBase;
using System;

namespace FirebaseSharp
{
    public class TestClass
    {
        public static void Main(string[] args)
        {
            var firebase = new Firebase();
            firebase.BaseUrl = "https://bitsonic-eb3f6.firebaseio.com";

            using (firebase.ListeningAsync<object>("/trade_histories/qtum-krw"))
            {
                string value = string.Empty;

                do
                {
                    value = Console.ReadLine();
                }
                while (!value.Equals("Done"));
            }
        }
    }
}
