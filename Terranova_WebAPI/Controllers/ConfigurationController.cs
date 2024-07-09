using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Terranova_APIClient;
using Terranova_GraphClient;

namespace Terranova_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private IOnBehalfOfUserFactory _onBehalfOfUserFactory;
        private readonly IAPIClient _apiClient;
        private ILogger Logger { get; }
        private IConfiguration _configuration;

        public ConfigurationController(IOnBehalfOfUserFactory onBehalfOfUserFactory, IAPIClient apiClient, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _onBehalfOfUserFactory = onBehalfOfUserFactory;
            _apiClient = apiClient;
            this.Logger = loggerFactory.CreateLogger("ConfigurationController");
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync(string envId, string locale)
        {
            try
            {
                var configResponse = await _apiClient.GetConfigurationAsync(envId, locale);
                if (configResponse.ConfigurationInfo != null)
                {
                    return Ok(configResponse.ConfigurationInfo);
                }
                else
                {
                    string message = configResponse.Detail;
                    foreach (var detail in configResponse.DetailError)
                    {
                        message += "\r\n";
                        if (!string.IsNullOrWhiteSpace(detail.FieldName))
                        {
                            message += $"{detail.FieldName}: ";
                        }
                        message += detail.ErrorDesc;
                    }
                    Logger.LogError("GetAsync: " + message);
                    return BadRequest(message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("GetAsync: " + ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("consentconfiguration")]
        public async Task<ActionResult> GetConsentConfiguration()
        {
            try
            {
                string graphAppId = _configuration.GetValue<string>("AzureAd:ClientId");
                string clientAppId = _configuration.GetValue<string>("ClientApp:ClientId");
                if (!string.IsNullOrEmpty(graphAppId) && !string.IsNullOrEmpty(clientAppId))
                {
                    return Ok(new { ClientAppId = clientAppId, GraphAppId = graphAppId });
                }
                return BadRequest("Configuration not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}