using FirebaseSharp.FireBase;
using System;

namespace FirebaseSharp
{
    public class TestClass
    {
        public static void Main(string[] args)
        {
            var firebase = new Firebase();
            firebase.BaseUrl = "";
            firebase.SecureToken = null;

            using (var listener = firebase.ListeningAsync<object>("/trade_histories/qtum-krw"))
            {

                listener.Wait();
                using (var f = listener.GetAwaiter().GetResult())
                {
                    string value = string.Empty;

                    do
                    {
                        value = Console.ReadLine();
                    }
                    while (!value.Equals("Done"));
                }
            }

            Console.WriteLine("Done");

            string values = string.Empty;
            do
            {
                values = Console.ReadLine();
            }
            while (!values.Equals("Done"));
        }
    }
}
