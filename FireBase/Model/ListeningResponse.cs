using FirebaseSharp.FireBase.Enums;
using Newtonsoft.Json; 

namespace FirebaseSharp.FireBase.Model
{
    public class FirebaseResponseData<T>
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }
        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }

    public class ListeningResponse
    {
        public EFirebaseRestMethod Type { get; set; }
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }
    }
}
