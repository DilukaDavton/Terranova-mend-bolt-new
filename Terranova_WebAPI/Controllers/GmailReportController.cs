using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Terranova_APIClient;
using Terranova_APIClient.Models;
using Terranova_GraphClient;

namespace Terranova_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GmailReportController : ControllerBase
    {
        private IAPIClient _apiClient;
        private ILogger Logger { get; }

        public GmailReportController( IAPIClient apiClient, ILoggerFactory loggerFactory)
        {
            _apiClient = apiClient;
            this.Logger = loggerFactory.CreateLogger("GmailReportController");
        }

        public async Task<ActionResult> PostAsync(string envUID, string twcId, string emailReportedBy)
        {
            try
            {
                var simulatingEmailInfo = new PhishingEmailInfo
                {
                    EnvUID = envUID,
                    TwcId = twcId,
                    EmailReportedBy = emailReportedBy,
                    EmailBody = "",
                    ReportedDate = DateTime.UtcNow.ToString(),
                    Comments = ""
                };
                //post to simulations API
                var responseFromSimulationReport = await _apiClient.ReportAsync(simulatingEmailInfo);
                if (responseFromSimulationReport.Status == "200")
                {
                    return Ok(new ApiResponse { FeedbackMessage = "Thank you", IsSimulation = true });
                }
                else
                {
                    return BadRequest(responseFromSimulationReport.Detail);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("PostAsync: " + ex.ToString());
                return BadRequest(ex.Message);
            }
        }
    }
}
