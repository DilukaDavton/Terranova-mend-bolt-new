using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terranova_APIClient;
using Terranova_APIClient.Models;
using Terranova_GraphClient;
using Recipient = Terranova_GraphClient.Models.Recipient;

namespace Terranova_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailReportController : ControllerBase
    {
        private IOnBehalfOfUserFactory _onBehalfOfUserFactory;
        private IAPIClient _apiClient;
        private IConfiguration _configuration;
        private ILogger Logger { get; }

        public EmailReportController(IOnBehalfOfUserFactory onBehalfOfUserFactory, IAPIClient apiClient, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _onBehalfOfUserFactory = onBehalfOfUserFactory;
            _apiClient = apiClient;
            _configuration = configuration;
            this.Logger = loggerFactory.CreateLogger("EmailReportController");
        }

        private string GetHeadersString(IEnumerable<InternetMessageHeader> headers)
        {
            var headerString = new StringBuilder();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    headerString.Append($"{header.Name}: {header.Value} {Environment.NewLine} {Environment.NewLine}");
                }
            }
            return headerString.ToString();
        }

        private SimulationInfo GetSimulationInfo(IEnumerable<InternetMessageHeader> internetMessageHeaders)
        {
            try
            {
                var twcHeader = internetMessageHeaders?.FirstOrDefault(x => x.Name.Equals("X-TWC", StringComparison.CurrentCultureIgnoreCase));
                if (twcHeader != null)
                {
                    //Get EnvUID
                    var twcEnv = internetMessageHeaders?.FirstOrDefault(x => x.Name.Equals("X-TWCE", StringComparison.CurrentCultureIgnoreCase));
                    //simulation
                    return new SimulationInfo { IsSimulation = true, TwcId = twcHeader.Value, TwcEnvUid = twcEnv?.Value };
                }
                else
                {
                    //phishing
                    return new SimulationInfo { IsSimulation = false };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("GetSimulationInfo: " + ex.ToString());
                throw;
            }

        }

        private bool IsMailInJunkOrDeletedItemsFolder(Message message, MailFolder junkMailFolder, MailFolder deletedItemsFolder)
        {
            var mailParentFolderId = message.ParentFolderId;
            var junkMailFolderId = junkMailFolder.Id;
            var deletetItemsFolderId = deletedItemsFolder.Id;
            if ((mailParentFolderId == junkMailFolderId) || (mailParentFolderId == deletetItemsFolderId)) return true;
            else return false;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("previouslyreportedstatus")]
        public async Task<ActionResult> GetPreviouslyReportedStatusAsync(PayloadInfo payload)
        {
            try
            {
                var bootstrapToken = HttpContext.User.Claims.FirstOrDefault().Subject.BootstrapContext.ToString();
                if (!string.IsNullOrEmpty(bootstrapToken))
                {
                    var exchangeClient = _onBehalfOfUserFactory.CreateClient(bootstrapToken);
                    if(payload.MailboxAddress != null && payload.MailItemId != null )
                    {
                       var response = await exchangeClient.GetPreviouslyReportedStatusAsync(payload.MailboxAddress, payload.MailItemId);
                       if (response != null)
                       {
                            if(response.SingleValueExtendedProperties != null)
                            {
                                var properties = response.SingleValueExtendedProperties.CurrentPage;
                                return properties != null && (properties.FirstOrDefault(v => v.Id == ExchangeClient._SingleValueExtendedPropertyName && v.Value == ExchangeClient._SingleValueExtendedPropertyValue) != null)
                                    ? Ok(true)
                                    : (ActionResult)Ok(false);
                            }
                            else
                            {
                                return Ok(false);
                            }
                       }
                       return BadRequest("Failed to get previous reported status");
                    }
                    else
                    {
                        return BadRequest("No mailbox address or mailitem provided");
                    }
                }
                else
                { return BadRequest("No token found"); }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("move")]
        public async Task<ActionResult> MoveAsync(PayloadInfo payload)
        {
            try
            {
                var bootstrapToken = HttpContext.User.Claims.FirstOrDefault().Subject.BootstrapContext.ToString();
                if (!string.IsNullOrEmpty(bootstrapToken))
                {
                    var exchangeClient = _onBehalfOfUserFactory.CreateClient(bootstrapToken);
                    if (payload.IsSimulation)
                    {
                        if (payload.ConfigurationInfo == null)
                        {
                            await exchangeClient.MoveEmailToDeletedItemsAsync(payload.MailboxAddress, payload.MailItemId);
                        }
                        else
                        {
                            if (payload.ConfigurationInfo.MoveSimulationEmailsToJunk)
                            {
                                await exchangeClient.MoveEmailToJunkAsync(payload.MailboxAddress, payload.MailItemId);
                            }
                            else
                            {
                                await exchangeClient.MoveEmailToDeletedItemsAsync(payload.MailboxAddress, payload.MailItemId);
                            }
                        }
                    }
                    else
                    {
                        //payload.ConfigurationInfo became null due to a caching issue on the front-end, this is done as a temp fix to prevent the addin from caching due to null ref error.
                        if (payload.ConfigurationInfo == null)
                        {
                            var envId = await _apiClient.GetEnvUidForEmailAsync(payload.MailboxAddress);
                            if (envId != null)
                            {
                                var configResponse = await _apiClient.GetConfigurationAsync(envId.ToString(), "en-US");
                                payload.ConfigurationInfo = configResponse.ConfigurationInfo;
                            }
                        }
                        if (payload.ConfigurationInfo == null)
                        {
                            //if above attempt to obtain ConfigurationInfo does not succeed.
                            await exchangeClient.MoveEmailToJunkAsync(payload.MailboxAddress, payload.MailItemId);
                        }
                        else
                        {
                            if (payload.ConfigurationInfo.MoveNonSimulationEmailsToJunk)
                            {
                                await exchangeClient.MoveEmailToJunkAsync(payload.MailboxAddress, payload.MailItemId);
                            }
                            else
                            {
                                await exchangeClient.MoveEmailToDeletedItemsAsync(payload.MailboxAddress, payload.MailItemId);
                            }
                        }
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest("No Token Found");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("MoveAsync: " + ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> PostAsync(PayloadInfo payload)
        {
            try
            {
                var bootstrapToken = HttpContext.User.Claims.FirstOrDefault().Subject.BootstrapContext.ToString();
                if (!string.IsNullOrEmpty(bootstrapToken))
                {
                    var exchangeClient = _onBehalfOfUserFactory.CreateClient(bootstrapToken);
                    var mailItemId = payload.MailItemId;
                    var mailBoxAddress = payload.MailboxAddress;
                    var environmentId = payload.EnvironmentId;
                    var comments = payload.InputFromCommentBox;
                    var configInfo = payload.ConfigurationInfo;
                    var reportedDate = DateTime.Now.ToString("yyyy-MM-dd");
                    var email = await exchangeClient.GetOriginalEmailAsync(mailBoxAddress, mailItemId);
                    var subject = $"Report Phishing email - [{email.Subject}]";
                    var internetMessageHeaders = email.InternetMessageHeaders;
                    var simulationInfo = GetSimulationInfo(internetMessageHeaders);
                    var junkMailFolder = await exchangeClient.GetJunkMailFolderAsync(mailBoxAddress);
                    var deletedMailFolder = await exchangeClient.GetDeletedItemsFolderAsync(mailBoxAddress);
                    var IsMailInJunkOrDelete = IsMailInJunkOrDeletedItemsFolder(email, junkMailFolder, deletedMailFolder);

                    if(configInfo != null)
                    {
                        if (simulationInfo.IsSimulation)
                        {                        
                            //Simulation mail
                            if (configInfo.BccForwardingSimulatedEmail)
                            {
                                var recipients = new List<Recipient>();
                                if (!string.IsNullOrEmpty(configInfo.BccForwardingEmail))
                                {
                                    foreach (var addr in configInfo.BccForwardingEmail.Split(','))
                                    {
                                        recipients.Add(new Recipient() { EmailAddress = addr.Trim(), IsBcc = false });
                                    }
                                }
                                if (configInfo.ForwardEmailAsAttachment)
                                {
                                    await exchangeClient.ForwardEmailAsAttachmentAsync(mailBoxAddress, mailItemId, recipients, GetHeadersString(internetMessageHeaders), subject, configInfo.FormatTitleForOffice365SecurityReportSubmission, email.Subject, configInfo.AttachEmailWithOriginalTitle, true, payload.EmailTypeSelectionInfo?.value, payload.InputFromCommentBox);
                                }
                                else
                                {
                                    await exchangeClient.ForwardEmailAsync(mailBoxAddress, mailItemId, recipients);
                                }
                            }
                            if (simulationInfo.TwcEnvUid == null)
                            {
                                Logger.LogWarning("PostAsync: Old Email Reported from [" + environmentId + "]");
                            }
                            var simulatingEmailInfo = new PhishingEmailInfo
                            {
                                EnvUID = simulationInfo.TwcEnvUid ?? environmentId, //Get EnvUID from simulationInfo/headers for simulation emails
                                TwcId = simulationInfo.TwcId,
                                EmailReportedBy = mailBoxAddress,
                                EmailBody = email.Body.Content,
                                ReportedDate = reportedDate,
                                Comments = comments
                            };
                            //post to simulations API
                            var responseFromSimulationReport = await _apiClient.ReportAsync(simulatingEmailInfo);
                            if (responseFromSimulationReport.Status == "200")
                            {
                                await exchangeClient.SetReportedStatusAsync(mailBoxAddress, mailItemId);
                                return Ok(new ApiResponse { IsMailInJunkOrDeletedItemsFolder = IsMailInJunkOrDelete, FeedbackMessage = configInfo.DisplaySimulationReinforcementMessage ? configInfo.SimulationReportFeedbackMessage : configInfo.ReportFeedbackMessage, IsSimulation = simulationInfo.IsSimulation });
                            }
                            else
                            {
                                string message = responseFromSimulationReport.Detail;
                                if(responseFromSimulationReport.DetailError != null)
                                {
                                    foreach (var detail in responseFromSimulationReport.DetailError)
                                    {
                                        message += "\r\n";
                                        if (!string.IsNullOrWhiteSpace(detail.FieldName))
                                        {
                                            message += $"{detail.FieldName}: ";
                                        }
                                        message += detail.ErrorDesc;
                                    }
                                }
                                Logger.LogError("ReportAsync (sim): " + message);
                                return BadRequest(message);
                            }
                        }
                        else
                        {
                            //Real Phishing mail
                            if (configInfo.IncidentResponseEnabled)
                            {
                                var recipients = new List<Recipient>();
                                if (!string.IsNullOrEmpty(configInfo.BccForwardingEmail) && configInfo.BccForwardingNonSimulatedEmail)
                                {
                                    foreach (var addr in configInfo.BccForwardingEmail.Split(','))
                                    {
                                        recipients.Add(new Recipient() { EmailAddress = addr.Trim(), IsBcc = true });
                                    }
                                }
                                if (configInfo.MicrosoftReportingEnabled)
                                {
                                    var microsoftPhishingReportMail = _configuration.GetValue<string>("MicrosoftReportingInfo:PhishingMail");
                                    recipients.Add(new Recipient() { EmailAddress = microsoftPhishingReportMail, IsBcc = true });
                                }
                                if (!string.IsNullOrWhiteSpace(configInfo.IncidentResponseEmail))
                                {
                                    foreach (var addr in configInfo.IncidentResponseEmail.Split(','))
                                    {
                                        recipients.Add(new Recipient() { EmailAddress = addr.Trim(), IsBcc = false });
                                    }
                                }
                                if (configInfo.ForwardEmailAsAttachment)
                                {
                                    await exchangeClient.ForwardEmailAsAttachmentAsync(mailBoxAddress, mailItemId, recipients, GetHeadersString(internetMessageHeaders), subject, configInfo.FormatTitleForOffice365SecurityReportSubmission, email.Subject, configInfo.AttachEmailWithOriginalTitle, true, payload.EmailTypeSelectionInfo?.value, payload.InputFromCommentBox);
                                }
                                else
                                {
                                    await exchangeClient.ForwardEmailAsync(mailBoxAddress, mailItemId, recipients);
                                }
                            }
                            var realPhishingEmailInfo = new PhishingEmailInfo
                            {
                                EnvUID = environmentId, //Get EnvUID from payload for real phishing emails
                                EmailReportedBy = mailBoxAddress,
                                EmailBody = email.Body.Content,
                                ReportedDate = reportedDate,
                                Comments = comments,
                                IsRealPhishing = true
                            };
                            var responseFromRealPhishingReport = await _apiClient.ReportAsync(realPhishingEmailInfo);
                            if (responseFromRealPhishingReport.DeserializedPayload != null)
                            {
                                await exchangeClient.SetReportedStatusAsync(mailBoxAddress, mailItemId);
                                return Ok(new ApiResponse { IsMailInJunkOrDeletedItemsFolder = IsMailInJunkOrDelete, FeedbackMessage = configInfo.ReportFeedbackMessage, IsSimulation = simulationInfo.IsSimulation });
                            }
                            else
                            {
                                string message = responseFromRealPhishingReport.Detail;
                                if(responseFromRealPhishingReport.DetailError != null)
                                {
                                    foreach (var detail in responseFromRealPhishingReport.DetailError)
                                    {
                                        message += "\r\n";
                                        if (!string.IsNullOrWhiteSpace(detail.FieldName))
                                        {
                                            message += $"{detail.FieldName}: ";
                                        }
                                        message += detail.ErrorDesc;
                                    }
                                }
                                Logger.LogError("ReportAsync (real): " + message);
                                return BadRequest(message);
                            }
                        }
                    }
                    else
                    {
                        return BadRequest("No Configuration found, please clear the cache and try again");
                    }
                }
                else
                {
                    return BadRequest("No Token Found");
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
