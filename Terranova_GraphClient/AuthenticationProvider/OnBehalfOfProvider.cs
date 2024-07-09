using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Terranova_GraphClient.AuthenticationProvider
{
    public class OnBehalfOfProvider : IAuthenticationProvider
	{
		/// <summary>
		///  A bootstrap token property
		/// </summary>
		public string BootstrapToken { get; set; }
		/// <summary>
		///  A Scope property
		/// </summary>
		public List<string> Scopes { get; set; }
		/// <summary>
		/// A <see cref="IConfidentialClientApplication"/> property.
		/// </summary>
		public IConfidentialClientApplication ClientApplication { get; set; }

		/// <summary>
		/// Constructs a new <see cref=" ClientCredentialProvider"/>
		/// </summary>
		/// <param name="confidentialClientApplication">A <see cref="IConfidentialClientApplication"/> to pass to <see cref="ClientCredentialProvider"/> for authentication.</param>
		/// <param name="scope">Scope required to access Microsoft Graph. This defaults to https://graph.microsoft.com/.default when none is set.</param>
		public OnBehalfOfProvider(IConfidentialClientApplication confidentialClientApplication, string bootstrapToken, List<string> scopes)
		{
			Scopes = scopes != null ? scopes : throw new AuthenticationException(
					new Error
					{
						Code = ErrorConstants.Codes.InvalidRequest,
						Message = string.Format(ErrorConstants.Message.NullValue, nameof(scopes))
					});
			BootstrapToken = !string.IsNullOrEmpty(bootstrapToken) ? bootstrapToken : throw new AuthenticationException(
					new Error
					{
						Code = ErrorConstants.Codes.InvalidRequest,
						Message = string.Format(ErrorConstants.Message.NullValue, nameof(bootstrapToken))
					});
			ClientApplication = confidentialClientApplication ?? throw new AuthenticationException(
					new Error
					{
						Code = ErrorConstants.Codes.InvalidRequest,
						Message = string.Format(ErrorConstants.Message.NullValue, nameof(confidentialClientApplication))
					});
		}

		/// <summary>
		/// Adds an authentication header to the incoming request by checking the application's <see cref="TokenCache"/>
		/// for an unexpired access token. If a token is not found or expired, it gets a new one.
		/// </summary>
		/// <param name="httpRequestMessage">A <see cref="HttpRequestMessage"/> to authenticate</param>
		public async Task AuthenticateRequestAsync(HttpRequestMessage httpRequestMessage)
		{
			AuthenticationProviderOption msalAuthProviderOption = httpRequestMessage.GetMsalAuthProviderOption();
			int retryCount = 0;
			do
			{
				try
				{
					var userAssertion = new UserAssertion(BootstrapToken);
					AuthenticationResult authenticationResult = await ClientApplication.AcquireTokenOnBehalfOf(Scopes, userAssertion)
						.ExecuteAsync();

					if (!string.IsNullOrEmpty(authenticationResult?.AccessToken))
					{
						httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(CoreConstants.Headers.Bearer, authenticationResult.AccessToken);
					}
					break;
				}
				catch (MsalUiRequiredException uiRequiredException) when (uiRequiredException.ErrorCode == MsalError.InvalidGrantError)
				{
					if (uiRequiredException.Classification == UiRequiredExceptionClassification.ConsentRequired)
					{
						throw new AuthenticationException(
							new Error
							{
								Code = ErrorConstants.Codes.AuthenticationException,
								Message = ErrorConstants.Message.ConsentRequired
							},
							uiRequiredException,
							AuthenticationExceptionClassification.ConsentRequired);
					}
					else
					{
						throw new AuthenticationException(
							new Error
							{
								Code = ErrorConstants.Codes.AuthenticationException,
								Message = ErrorConstants.Message.UnexpectedMsalException
							},
							uiRequiredException);
					}

				}
				catch (MsalServiceException serviceException)
				{
					if (serviceException.ErrorCode == ErrorConstants.Codes.TemporarilyUnavailable)
					{
						TimeSpan delay = this.GetRetryAfter(serviceException);
						retryCount++;
						// pause execution
						await Task.Delay(delay);
					}
					else
					{
						throw new AuthenticationException(
							new Error
							{
								Code = ErrorConstants.Codes.AuthenticationException,
								Message = ErrorConstants.Message.UnexpectedMsalException
							},
							serviceException);
					}
				}
				catch (Exception exception)
				{
					throw new AuthenticationException(
							new Error
							{
								Code = ErrorConstants.Codes.AuthenticationException,
								Message = ErrorConstants.Message.UnexpectedException
							},
							exception);
				}

			} while (retryCount < msalAuthProviderOption.MaxRetry);
		}
	}
}
