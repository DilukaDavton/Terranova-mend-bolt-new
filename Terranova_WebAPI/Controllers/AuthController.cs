using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Terranova_APIClient.Models;
using Terranova_DBClient;
using Terranova_GraphClient;
using Terranova_GraphClient.AuthenticationProvider;
using Terranova_WebAPI.Helpers;
using Terranova_WebAPI.Models;

namespace Terranova_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IOnBehalfOfUserFactory _onBehalfOfUserFactory;
        private readonly IDBClient _dbClient;
        private readonly IConfiguration _configuration;
        private ILogger Logger { get; }

        public AuthController(IOnBehalfOfUserFactory onBehalfOfUserFactory, IConfiguration configuration, ILoggerFactory loggerFactory, IDBClient dbClient)
        {
            _onBehalfOfUserFactory = onBehalfOfUserFactory;
            _configuration = configuration;
            this.Logger = loggerFactory.CreateLogger("AuthController");
            _dbClient = dbClient;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return Ok("OK");
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> PostAsync()
        {
            try
            {
                var bootstrapToken = HttpContext.User.Claims.FirstOrDefault().Subject.BootstrapContext.ToString();
                if (!string.IsNullOrEmpty(bootstrapToken))
                {
                    var exchangeClient = _onBehalfOfUserFactory.CreateClient(bootstrapToken);
                    var response = await exchangeClient.GetUserAsync();
                    if (response != null)
                        return Ok();
                    else
                        return BadRequest("No User Found");
                }
                else
                {
                    return BadRequest("No Token Found");
                }
            }
            catch (AuthenticationException authException)
            {
                Logger.LogError("PostAsync(AuthenticationException): " + authException.ToString());
                return BadRequest(JsonConvert.SerializeObject(authException.InnerException));
            }
            catch (Exception ex)
            {
                Logger.LogError("PostAsync: " + ex.ToString());
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("token")]
        public async Task<ActionResult> RetrieveTokenAsync(TokenRequest requestInfo)
        {
            var interfaceResponse = new InterfaceResponse();
            try
            {
                if (!string.IsNullOrWhiteSpace(requestInfo.Code))
                {
                    var token = await new AuthClient(_configuration).GetTokenAsync(requestInfo.Code);
                    var responseData = new TokenViewModel()
                    {
                        AccessToken = token.access_token,
                        RefreshToken = token.refresh_token
                    };
                    interfaceResponse.TokenInfo = responseData;
                    return Ok(interfaceResponse);
                }
                else
                {
                    interfaceResponse.IsError = true;
                    interfaceResponse.ErrorMessage = "Request code is not found...";
                    return BadRequest(interfaceResponse);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("RetrieveTokenAsync: " + ex.ToString());
                interfaceResponse.IsError = true;
                interfaceResponse.ErrorMessage = ex.Message;
                return BadRequest(interfaceResponse);
            }
        }

        [HttpPost("save-token")]
        public async Task<ActionResult> SaveTokenAsync(TokenSaveRequestInfo requestInfo)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(requestInfo.Code))
                {
                    var token = await new AuthClient(_configuration).GetTokenForIntuneConfiguredEnvAsync(requestInfo.Code);
                    if (!string.IsNullOrEmpty(requestInfo.TokenRetrievalGuid))
                    {
                        //save token in to Azure table storage
                        await _dbClient.SaveRefreshTokenAsync(token.refresh_token, requestInfo.TokenRetrievalGuid);
                        return Ok();

                    }
                    else
                    {
                        return BadRequest("TokenRetrievalGuid is not found");
                    }
                }
                else
                {
                    return BadRequest("Code is not found");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("SaveTokenAsync: " + ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshTokenAsync(RefreshTokenRequest requestInfo)
        {
            var interfaceResponse = new InterfaceResponse();
            try
            {
                if (!string.IsNullOrWhiteSpace(requestInfo.RefreshToken))
                {
                    var token = await new AuthClient(_configuration).RenewTokenAsync(requestInfo.RefreshToken);
                    var responseData = new TokenViewModel()
                    {
                        AccessToken = token.access_token,
                        RefreshToken = token.refresh_token
                    };
                    interfaceResponse.TokenInfo = responseData;
                    return Ok(interfaceResponse);
                }
                else
                {
                    interfaceResponse.IsError = true;
                    interfaceResponse.ErrorMessage = "Refresh Token is not found...";
                    return BadRequest(interfaceResponse);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("RefreshTokenAsync: " + ex.ToString());
                interfaceResponse.IsError = true;
                interfaceResponse.ErrorMessage = ex.Message;
                return BadRequest(interfaceResponse);
            }
        }


        [HttpGet("url")]
        public async Task<ActionResult> GetAuthUrl(string tokenRetrievalGuid, bool isIntuneConfigured)
        {
            var url = await new AuthClient(_configuration).GetLoginURLAsync(tokenRetrievalGuid, isIntuneConfigured);
            return Ok(url);
        }

        [HttpGet("refresh-token-for-guid")]
        public async Task<ActionResult> GetRefreshTokenByTokenRetrievalGuid(string tokenRetrievalGuid)
        {
            if (string.IsNullOrEmpty(tokenRetrievalGuid))
            {
                return BadRequest("tokenRetrievalGuid is null or empty");
            }
            try
            {
                var refreshTokenInfo = await _dbClient.GetRefreshTokenAsync(tokenRetrievalGuid);
                return Ok(refreshTokenInfo);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

    }
}
