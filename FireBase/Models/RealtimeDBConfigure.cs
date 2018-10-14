using Newtonsoft.Json;

namespace FirebaseSharp.Firebase.Models
{
    public struct ReatimeDBConfigure
    {
        [JsonProperty(PropertyName = "apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty(PropertyName = "authDomain")]
        public string AuthDomain { get; set; }

        [JsonProperty(PropertyName = "databaseURL")]
        public string DatabaseURL { get; set; }

        [JsonProperty(PropertyName = "projectId")]
        public string ProjectId { get; set; }

        [JsonProperty(PropertyName = "storageBucket")]
        public string StorageBucket { get; set; }

        [JsonProperty(PropertyName = "messagingSenderId")]
        public string MessageSenderId { get; set; }
    }
}
