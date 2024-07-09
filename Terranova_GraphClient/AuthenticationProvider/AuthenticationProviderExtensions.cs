using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Terranova_GraphClient.AuthenticationProvider
{
    internal static class AuthenticationProviderExtensions
    {
		/// <summary>
		/// Gets retry after timespan from <see cref="RetryConditionHeaderValue"/>.
		/// </summary>
		/// <param name="authProvider">An <see cref="IAuthenticationProvider"/> object.</param>
		/// <param name="serviceException">A <see cref="MsalServiceException"/> with RetryAfter header</param>
		internal static TimeSpan GetRetryAfter(this IAuthenticationProvider authProvider, MsalServiceException serviceException)
		{
			RetryConditionHeaderValue retryAfter = serviceException.Headers?.RetryAfter;
			TimeSpan? delay = null;

			if (retryAfter != null && retryAfter.Delta.HasValue)
			{
				delay = retryAfter.Delta;
			}
			else if (retryAfter != null && retryAfter.Date.HasValue)
			{
				delay = retryAfter.Date.Value.Offset;
			}

			if (delay == null)
			{
				throw new MsalServiceException(serviceException.ErrorCode, ErrorConstants.Message.MissingRetryAfterHeader);
			}

			return delay.Value;
		}
	}
}
