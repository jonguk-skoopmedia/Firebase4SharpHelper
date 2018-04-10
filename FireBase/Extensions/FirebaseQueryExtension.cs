using System.Collections.Generic;

namespace lvChartTest.Util.FireBase.Extensions
{
    public static class FirebaseQueryExtension
    {
        public static Dictionary<string, string> AddQuery(this Dictionary<string, string> pair, string query, string parameter)
        {
            pair.Add(query, parameter);

            return pair;
        }
    }
}
