using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Terranova_APIClient;
using Terranova_GraphClient;

namespace Terranova_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiInfoController : ControllerBase
    {
        private IOnBehalfOfUserFactory _onBehalfOfUserFactory;
        private readonly IAPIClient _apiClient;
        private ILogger Logger { get; }

        public ApiInfoController(IOnBehalfOfUserFactory onBehalfOfUserFactory, IAPIClient apiClient, ILoggerFactory loggerFactory)
        {
            _onBehalfOfUserFactory = onBehalfOfUserFactory;
            _apiClient = apiClient;
            this.Logger = loggerFactory.CreateLogger("ApiInfoController");
        }

        [HttpGet]
        public ActionResult Get(string EnvUID)
        {
            if (Request.Headers.TryGetValue("Authorization", out var authorization))
            {
                string token = authorization.ToString();

                if (token.Contains(Startup.Token))
                {
                    try
                    {
                        return Ok(EnvCache.GetReportApiInfo(_apiClient.ReportApiInfos, EnvUID, Logger));
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Get: " + ex.ToString());
                        return BadRequest(ex.Message);
                    }
                }
                else
                {
                    Logger.LogError("Wrong Token: " + token);
                }
            }

            return Forbid();
        }

        [HttpGet]
        [Route("UidForEmail")]
        public async Task<ActionResult> UidForEmail(string email)
        {
            try
            {
                return Ok(await _apiClient.GetEnvUidForEmailAsync(email));
            }
            catch (Exception ex)
            {
                Logger.LogError("Get: " + ex.ToString());
                return BadRequest(ex.Message);
            }
        }
    }
}
