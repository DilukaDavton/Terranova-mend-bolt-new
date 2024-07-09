using Newtonsoft.Json;
using System.Collections.Generic;

namespace Terranova_APIClient.Models
{
    public class ApiResponse
    {
        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("detailError")]
        public List<DetailError> DetailError { get; set; }

        [JsonProperty("payload")]
        public string Payload { get; set; }

        [JsonProperty("userExist")]
        public bool IsUserExist { get; set; }

        public bool IsMailInJunkOrDeletedItemsFolder { get; set; }
        public string FeedbackMessage { get; set; }
        public bool IsSimulation { get; set; }
        public PhishingEmailInfo DeserializedPayload { get; set; }
        public ConfigurationInfo ConfigurationInfo { get; set; }

    }
}
