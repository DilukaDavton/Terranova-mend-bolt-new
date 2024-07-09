using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Terranova_WebAPI.Models;

namespace Terranova_WebAPI.Helpers
{
    public class AuthClient
    {
        private IConfiguration _configuration;

        public AuthClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Obtain the access token for the mobile clients which follow standard auth code flow (Intune has not configured)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<MSTokenResponse> GetTokenAsync(string code)
        {
            MSTokenResponse msTokenResponse = null;
            var content = "grant_type=authorization_code&" +
                            "scope=" + _configuration.GetValue<string>("ClientApp:Scope") + "&" +
                            "code=" + code + "&" +
                            "client_id=" + _configuration.GetValue<string>("ClientApp:ClientId") + "&" +
                            "client_secret=" + _configuration.GetValue<string>("ClientApp:ClientSecret") + "&" +
                            "redirect_uri="+ _configuration.GetValue<string>("ClientApp:RedirectUrl");
            if (!string.IsNullOrEmpty(code))
            {
                var authResp = await new WebHelper().HttpPostAsync(_configuration.GetValue<string>("ClientApp:TokenEndpoint"), content);
                msTokenResponse = JsonConvert.DeserializeObject<MSTokenResponse>(authResp);
            }
            return msTokenResponse;
        }

        /// <summary>
        /// Obtain the access token for the mobile clients which are configured with Intune
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<MSTokenResponse> GetTokenForIntuneConfiguredEnvAsync(string code)
        {
            MSTokenResponse msTokenResponse = null;
            var content = "grant_type=authorization_code&" +
                            "scope=" + _configuration.GetValue<string>("ClientApp:Scope") + "&" +
                            "code=" + code + "&" +
                            "client_id=" + _configuration.GetValue<string>("ClientApp:ClientId") + "&" +
                            "client_secret=" + _configuration.GetValue<string>("ClientApp:ClientSecret") + "&" +
                            "redirect_uri=" + _configuration.GetValue<string>("ClientApp:RedirectUrlForIntuneConfiguredDevices");
            if (!string.IsNullOrEmpty(code))
            {
                var authResp = await new WebHelper().HttpPostAsync(_configuration.GetValue<string>("ClientApp:TokenEndpoint"), content);
                msTokenResponse = JsonConvert.DeserializeObject<MSTokenResponse>(authResp);
            }
            return msTokenResponse;
        }

        public async Task<MSTokenResponse> RenewTokenAsync(string refreshToken)
        {
            MSTokenResponse msTokenResponse = null;
            var content = "grant_type=refresh_token&" +
                             "refresh_token=" + refreshToken + "&" +
                             "client_id=" + _configuration.GetValue<string>("ClientApp:ClientId") + "&" +
                             "client_secret=" + _configuration.GetValue<string>("ClientApp:ClientSecret");

            var authResp = await new WebHelper().HttpPostAsync(_configuration.GetValue<string>("ClientApp:TokenEndpoint"), content);
            msTokenResponse = JsonConvert.DeserializeObject<MSTokenResponse>(authResp);
            return msTokenResponse;
        }

        public async Task<string> GetLoginURLAsync(string tokenRetrievalGuid, bool isIntuneConfigured)
        {
            if (isIntuneConfigured)
            {
                return "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?" +
                     "client_id=" + _configuration.GetValue<string>("ClientApp:ClientId") + "&" +
                     "response_mode=query&" +
                     "state=" + tokenRetrievalGuid + "&" +
                     "response_type=code&" +
                     "scope=" + _configuration.GetValue<string>("ClientApp:Scope") + "&" +
                     "redirect_uri=" + _configuration.GetValue<string>("ClientApp:RedirectUrlForIntuneConfiguredDevices");
            }
            else
            {
                return "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?" +
                     "client_id=" + _configuration.GetValue<string>("ClientApp:ClientId") + "&" +
                     "response_mode=query&" +
                     "state=" +"calledfromdialog" + "&" +
                     "response_type=code&" +
                     "scope=" + _configuration.GetValue<string>("ClientApp:Scope") + "&" +
                     "redirect_uri=" + _configuration.GetValue<string>("ClientApp:RedirectUrl");
            }
        }
    }
}



