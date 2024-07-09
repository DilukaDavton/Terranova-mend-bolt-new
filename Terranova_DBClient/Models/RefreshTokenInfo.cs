using Microsoft.WindowsAzure.Storage.Table;

namespace Terranova_DBClient.Models
{
    public class RefreshTokenInfo: TableEntity
    {
        public string TokenRetrievalGuid { get; set; }
        public string RefreshToken { get; set; }

        public RefreshTokenInfo(string tokenRetrievalGuid)
        {
            PartitionKey = tokenRetrievalGuid;
            RowKey = tokenRetrievalGuid;
        }

        public RefreshTokenInfo()
        {

        }
    }
}
