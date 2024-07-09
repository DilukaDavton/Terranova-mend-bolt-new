using Newtonsoft.Json;

namespace Terranova_APIClient.Models
{
    public class PhishingEmailInfo
    {
        [JsonProperty("TwcId")]
        public string TwcId { get; set; }

        [JsonProperty("EnvUID")]
        public string EnvUID { get; set; }

        [JsonProperty("EmailReportedBy")]
        public string EmailReportedBy { get; set; }

        [JsonProperty("EmailBody")]
        public string EmailBody { get; set; }

        [JsonProperty("ReportedDate")]
        public string ReportedDate { get; set; }

        [JsonProperty("Comments")]
        public string Comments { get; set; }

        [JsonProperty("IsRealPhishing")]
        public bool IsRealPhishing { get; set; }
    }
}
