using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Terranova_APIClient;

namespace Terranova_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GmailConfigurationController : ControllerBase
    {
        private readonly IAPIClient _apiClient;
        private ILogger Logger { get; }

        public GmailConfigurationController(IAPIClient apiClient, ILoggerFactory loggerFactory)
        {
            _apiClient = apiClient;
            this.Logger = loggerFactory.CreateLogger("GmailConfigurationController");
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync(string envUID, string locale)
        {
            try
            {
                var configResponse = await _apiClient.GetGmailConfigurationAsync(envUID, locale);
                if (configResponse.ConfigurationInfo != null)
                {
                    return Ok(configResponse.ConfigurationInfo);
                }
                else
                {
                    return BadRequest(configResponse.Detail);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("GetAsync: " + ex.ToString());
                return BadRequest(ex.Message);
            }
        }
    }
}