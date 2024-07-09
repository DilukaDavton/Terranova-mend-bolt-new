namespace Terranova_APIClient.Models
{
    public class PayloadInfo
    {
        public string MailItemId { get; set; }
        public string InputFromCommentBox { get; set; }
        public string EnvironmentId { get; set; }
        public string MailboxAddress { get; set; }
        public TypeSelectionInfo EmailTypeSelectionInfo { get; set; }
        public ConfigurationInfo ConfigurationInfo { get; set; }
        public bool IsSimulation { get; set; }
    }
}
