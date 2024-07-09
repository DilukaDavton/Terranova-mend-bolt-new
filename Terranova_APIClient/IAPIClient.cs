using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Terranova_APIClient.Models;

namespace Terranova_APIClient
{
    public interface IAPIClient
    {
        Dictionary<int, ReportApiInfo> ReportApiInfos { get; }
        Task<ApiResponse> ReportAsync(PhishingEmailInfo emailInfo);
        Task<ApiResponse> GetConfigurationAsync(string environmentId, string locale);
        Task<Guid> GetEnvUidForEmailAsync(string email);
        Task<ApiResponse> GetGmailConfigurationAsync(string environmentId, string locale);
    }
}
