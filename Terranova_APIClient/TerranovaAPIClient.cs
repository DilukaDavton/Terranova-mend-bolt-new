using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Terranova_APIClient.Models;

namespace Terranova_APIClient
{
    public class TerranovaAPIClient : IAPIClient
    {
        public Dictionary<int, ReportApiInfo> ReportApiInfos { get; }
        private ILogger Logger { get; }

        public TerranovaAPIClient(Dictionary<int, ReportApiInfo> reportApiInfos, ILoggerFactory loggerFactory)
        {
            ReportApiInfos = reportApiInfos;
            this.Logger = loggerFactory.CreateLogger("TerranovaAPIClient");
        }

        private HttpClient GetHttpClient(string envUID, out ReportApiInfo reportApiInfo)
        {
            reportApiInfo = EnvCache.GetReportApiInfo(ReportApiInfos, envUID, Logger);
            if (reportApiInfo == null)
            {                              
                Logger.LogError("GetReportApiInfo: Uid not found[" + envUID + "]");
                throw new Exception("The environment id provided is not found in the Terranova Portal.");               
            }
            return GetHttpClient(reportApiInfo);
        }

        private static HttpClient GetHttpClient(ReportApiInfo apiInfo)
        {
            if (apiInfo != null)
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(apiInfo.ApiUrl)
                };
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiInfo.BearerToken);
                ServicePointManager.Expect100Continue = true;
                return client;
            }
            return null;
        }

        public async Task<ApiResponse> ReportAsync(PhishingEmailInfo emailInfo)
        {
            try
            {
                var content = JsonConvert.SerializeObject(emailInfo);
                using (var client = GetHttpClient(emailInfo.EnvUID, out var reportApiInfo))
                {
                    var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(reportApiInfo.ApiUrl + "/api/ReportPhishing/", stringContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var resultContent = await response.Content.ReadAsStringAsync();
                        var deserializedResponse = JsonConvert.DeserializeObject<ApiResponse>(resultContent);
                        if (deserializedResponse.Payload != null)
                        {
                            deserializedResponse.DeserializedPayload = JsonConvert.DeserializeObject<PhishingEmailInfo>(deserializedResponse.Payload);
                        }
                        return deserializedResponse;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Logger.LogError("ReportAsync/FailedResponse: " + errorContent);
                        return JsonConvert.DeserializeObject<ApiResponse>(errorContent);
                    }
                }
            }
            catch (WebException ex)
            {
                Logger.LogError("ReportAsync: " + ex.ToString());
                return new ApiResponse
                {
                    Status = ex.Status.ToString()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Status = "500",
                    Detail = ex.Message
                };
            }
        }

        public async Task<ApiResponse> GetConfigurationAsync(string environmentId, string locale)
        {
            try
            {
                var configRequest = new ConfigRequest
                {
                    CodeCulture = locale,
                    EnvUID = environmentId
                };
                using (var client = GetHttpClient(environmentId, out var reportApiInfo))
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        Content = new StringContent(JsonConvert.SerializeObject(configRequest), Encoding.UTF8, "application/json"),
                        RequestUri = new Uri(reportApiInfo.ApiUrl + "/api/ReportPhishing/")
                    };
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var resultContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var deserializedResponse = JsonConvert.DeserializeObject<ConfigurationInfo>(resultContent);
                        //Temporary code until implemented in platerrra APIs
                        if (string.IsNullOrWhiteSpace(deserializedResponse.MoveToDeletedItemsButtonCaption))
                        {
                            deserializedResponse.MoveToDeletedItemsButtonCaption = deserializedResponse.MoveToJunkButtonCaption?.Replace("junk", "deleted items");
                        }
                        return new ApiResponse
                        {
                            ConfigurationInfo = deserializedResponse
                        };
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<ApiResponse>(errorContent);
                    }
                }
            }
            catch (WebException ex)
            {
                Logger.LogError("GetConfigurationAsync: " + ex.ToString());
                return new ApiResponse
                {
                    Status = ex.Status.ToString(),
                    Detail = ex.Message,
                    DetailError = new List<DetailError>() {
                        new DetailError() {
                            FieldName = "environmentId",
                            ErrorDesc = environmentId ?? string.Empty
                        },
                        new DetailError() {
                            FieldName = "locale",
                            ErrorDesc = locale ?? string.Empty
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Status = "500",
                    Detail = ex.Message,
                    DetailError = new List<DetailError>() {
                        new DetailError() {
                            FieldName = "environmentId",
                            ErrorDesc = environmentId ?? string.Empty
                        },
                        new DetailError() {
                            FieldName = "locale",
                            ErrorDesc = locale ?? string.Empty
                        }
                    }
                };
            }
        }
      
        public async Task<ApiResponse> GetGmailConfigurationAsync(string environmentId, string locale)
        {
            try
            {
                var configRequest = new ConfigRequest
                {
                    CodeCulture = locale,
                    EnvUID = environmentId
                };
                using (var client = GetHttpClient(environmentId, out var reportApiInfo))
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        Content = new StringContent(JsonConvert.SerializeObject(configRequest), Encoding.UTF8, "application/json"),
                        RequestUri = new Uri(reportApiInfo.ApiUrl + "/api/ReportPhishing/")
                    };
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var resultContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var deserializedResponse = JsonConvert.DeserializeObject<ConfigurationInfo>(resultContent);
                        //Temporary code until implemented in platerrra APIs
                        if (string.IsNullOrWhiteSpace(deserializedResponse.MoveToDeletedItemsButtonCaption))
                        {
                            deserializedResponse.MoveToDeletedItemsButtonCaption = deserializedResponse.MoveToJunkButtonCaption?.Replace("junk", "deleted items");
                        }
                        return new ApiResponse
                        {
                            ConfigurationInfo = deserializedResponse
                        };
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<ApiResponse>(errorContent);
                    }
                }
            }
            catch (WebException ex)
            {
                Logger.LogError("GetGmailConfigurationAsync: " + ex.ToString());
                return new ApiResponse
                {
                    Status = ex.Status.ToString(),
                    Detail = ex.Message,
                    DetailError = new List<DetailError>() {
                        new DetailError() {
                            FieldName = "environmentId",
                            ErrorDesc = environmentId ?? string.Empty
                        },
                        new DetailError() {
                            FieldName = "locale",
                            ErrorDesc = locale ?? string.Empty
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Status = "500",
                    Detail = ex.Message,
                    DetailError = new List<DetailError>() {
                        new DetailError() {
                            FieldName = "environmentId",
                            ErrorDesc = environmentId ?? string.Empty
                        },
                        new DetailError() {
                            FieldName = "locale",
                            ErrorDesc = locale ?? string.Empty
                        }
                    }
                };
            }
        }

        public static async Task<List<Guid>> GetUidsAsync(ReportApiInfo reportApiInfo, ILogger logger)
        {
            try
            {
                using (var client = GetHttpClient(reportApiInfo))
                {
                    if (client != null)
                    {
                        var response = await client.GetAsync(reportApiInfo.ApiUrl + "/Api/Environment/UidList/");
                        if (response.IsSuccessStatusCode)
                        {
                            var resultContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            return JsonConvert.DeserializeObject<List<Guid>>(resultContent);
                        }
                        else
                        {
                            logger.LogError($"GetUidsAsync[{reportApiInfo.ApiUrl}]: {response.StatusCode}: {response.Content}");
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                logger.LogError("GetUidsAsync: " + ex.ToString());
            }
            return new List<Guid>();
        }

        public async Task<Guid> GetEnvUidForEmailAsync(string email)
        {
            try
            {
                foreach (var info in ReportApiInfos)
                {
                    //Return first envuid found for given email
                    var guid = await GetUidForEmailAsync(info.Value, email);
                    if (guid != Guid.Empty)
                    {
                        return guid;
                    }
                }
            }
            catch (WebException ex)
            {
                Logger.LogError("GetEnvUidForEmailAsync: " + ex.ToString());
            }
            return Guid.Empty;
        }

        private async Task<Guid> GetUidForEmailAsync(ReportApiInfo reportApiInfo, string email)
        {
            try
            {
                using (var client = GetHttpClient(reportApiInfo))
                {
                    if (client != null)
                    {
                        var response = await client.GetAsync(reportApiInfo.ApiUrl + "/Api/Environment/GetForEmail/?email=" + email);
                        if (response.IsSuccessStatusCode)
                        {
                            var resultContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                            try
                            {
                                return JsonConvert.DeserializeObject<Guid>(resultContent);
                            }
                            catch (Exception)
                            {
                                return JsonConvert.DeserializeObject<List<Guid>>(resultContent).FirstOrDefault();
                            }
                        }
                        else
                        {
                            Logger.LogError($"GetUidForEmailAsync[{reportApiInfo.ApiUrl}]: {response.StatusCode}: {response.Content}");
                        }
                    }
                }
                //Not found
                return Guid.Empty;
            }
            catch (WebException ex)
            {
                Logger.LogError("GetUidForEmailAsync: " + ex.ToString());
            }
            return Guid.Empty;
        }
    }
}