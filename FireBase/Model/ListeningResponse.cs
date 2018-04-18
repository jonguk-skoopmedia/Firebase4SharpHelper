using FirebaseSharp.FireBase.Enums;
using Newtonsoft.Json; 

namespace FirebaseSharp.FireBase.Model
{
    public class ResponseData<T>
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }
        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }

    public class ListeningResponse<T>
    {
        public EFirebaseRestMethod Type { get; set; }
        [JsonProperty(PropertyName = "data")]
        public ResponseData<T> Data { get; set; }
    }
}
