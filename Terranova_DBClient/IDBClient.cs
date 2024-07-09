using System.Threading.Tasks;
using Terranova_DBClient.Models;

namespace Terranova_DBClient
{
    public interface IDBClient
    {
        Task SaveRefreshTokenAsync(string refreshToken, string tokenRetrievalGuid);
        Task<RefreshTokenInfo> GetRefreshTokenAsync(string tokenRetrievalGuid);
    }
}
