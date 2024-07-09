using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Terranova_GraphClient.AuthenticationProvider
{
    internal static class HttpRequestMessageExtensions
    {
        internal static AuthenticationProviderOption GetMsalAuthProviderOption(this HttpRequestMessage httpRequestMessage)
        {
            AuthenticationHandlerOption authHandlerOption = httpRequestMessage.GetMiddlewareOption<AuthenticationHandlerOption>();
            return authHandlerOption?.AuthenticationProviderOption as AuthenticationProviderOption ?? new AuthenticationProviderOption();
        }
    }
}
