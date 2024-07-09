using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using Terranova_GraphClient.AuthenticationProvider;
using Terranova_GraphClient.Settings;

namespace Terranova_GraphClient
{
    public class OnBehalfOfUserFactory : IOnBehalfOfUserFactory
	{
		private readonly IOptions<OBOSettings> _oboSettings;
		private readonly IConfidentialClientApplication _confidentialClientApp;
		private readonly GraphServiceClient _graphServiceClient;

        private const int _OverallTimeOut = 5;

        public OnBehalfOfUserFactory(IOptions<OBOSettings> settings, GraphServiceClient graphServiceClient)
		{
			_oboSettings = settings;
			_confidentialClientApp = ConfidentialClientApplicationBuilder.Create(_oboSettings.Value.ClientId)
					.WithClientSecret(_oboSettings.Value.ClientSecret)
					.Build();
			_graphServiceClient = graphServiceClient;
		}

		public IExchangeClient CreateClient(string bootstrapToken)
		{
			_graphServiceClient.AuthenticationProvider = new OnBehalfOfProvider(_confidentialClientApp, bootstrapToken, _oboSettings.Value.Scopes);
            _graphServiceClient.HttpProvider.OverallTimeout = TimeSpan.FromMinutes(_OverallTimeOut);
            return new ExchangeClient(_graphServiceClient);
		}
	}
}
