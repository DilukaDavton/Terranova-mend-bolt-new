using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;
using Terranova_DBClient.Models;

namespace Terranova_DBClient
{
    public class AzureDBClient : IDBClient
    {
        private IConfiguration _configuration;
        private static string _azureStorageConnectionString;
        private static string _tableName = "";

        public AzureDBClient(IConfiguration configuration)
        {
            _configuration = configuration;
            _azureStorageConnectionString = _configuration["AzureStorageConnectionString"];
            _tableName = _configuration["AzureTableName"];
        }

        public async Task SaveRefreshTokenAsync(string refreshToken, string tokenRetrievalGuid)
        {
            if(string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentNullException($"{nameof(refreshToken)} can not be null or empty");
            }
            if (string.IsNullOrEmpty(tokenRetrievalGuid))
            {
                throw new ArgumentNullException($"{nameof(tokenRetrievalGuid)} can not be null or empty");
            }
            try
            {
                var refreshTokenInfo = new RefreshTokenInfo(tokenRetrievalGuid) {
                    TokenRetrievalGuid = tokenRetrievalGuid,
                    RefreshToken = refreshToken
                };
                var insertOperation = TableOperation.Insert(refreshTokenInfo);
                var result = await GetTable().ExecuteAsync(insertOperation);
                var insertedEntity = result.Result as RefreshTokenInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static CloudTable GetTable()
        {
            var storageAccount = CloudStorageAccount.Parse(_azureStorageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference(_tableName);
        }

        public async Task<RefreshTokenInfo> GetRefreshTokenAsync(string tokenRetrievalGuid)
        {
            if (string.IsNullOrEmpty(tokenRetrievalGuid))
            {
                throw new ArgumentNullException($"{nameof(tokenRetrievalGuid)} can not be null or empty");
            }
            try
            {
                var rangeQuery = new TableQuery<RefreshTokenInfo>().Where(
                   TableQuery.CombineFilters(
                   TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, tokenRetrievalGuid),
                   TableOperators.And,
                   TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, tokenRetrievalGuid)));
                var response = await GetTable().ExecuteQuerySegmentedAsync(rangeQuery, null);
                if (response?.Results.Count == 0)
                {
                    return null;
                }
                else
                {
                    var deleteOperation = TableOperation.Delete(response.Results[0]);
                    var result = await GetTable().ExecuteAsync(deleteOperation);
                    return response.Results[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
