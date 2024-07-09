using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace Terranova_GraphClient.AuthenticationProvider
{
	/// <summary>
	/// Options class used to configure the authentication providers.
	/// </summary>
	public class AuthenticationProviderOption : IAuthenticationProviderOption
	{
		/// <summary>
		/// A MaxRetry property.
		/// </summary>
		internal int MaxRetry { get; set; } = 1;

		/// <summary>
		/// Scopes to use when authenticating.
		/// </summary>
		public string[] Scopes { get; set; }

		/// <summary>
		/// Whether or not to force a token refresh.
		/// </summary>
		public bool ForceRefresh { get; set; }
	}
}
