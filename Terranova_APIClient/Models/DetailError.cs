using Newtonsoft.Json;

namespace Terranova_APIClient.Models
{
    public class DetailError
    {
        [JsonProperty("fieldName")]
        public string FieldName { get; set; }

        [JsonProperty("errorDesc")]
        public string ErrorDesc { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }
    }
}
