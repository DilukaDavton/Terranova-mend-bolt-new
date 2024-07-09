using System;
using System.Collections.Generic;
using System.Text;

namespace Terranova_GraphClient.AuthenticationProvider
{
    public class AuthenticationException : Exception
    {
		/// <summary>
		/// Creates a new authentication exception.
		/// </summary>
		/// <param name="error">The error that triggered the exception.</param>
		/// <param name="innerException">The possible inner exception.</param>
		public AuthenticationException(Error error, Exception innerException = null)
			: base(error?.ToString(), innerException)
		{
			Error = error;
		}

		public AuthenticationException(Error error, Exception innerException, AuthenticationExceptionClassification authenticationExceptionClassification)
			: base(error?.ToString(), innerException)
		{
			Error = error;
			Classification = authenticationExceptionClassification;
		}

		/// <summary>
		/// The error from the authentication exception.
		/// </summary>
		public Error Error { get; private set; }

		public AuthenticationExceptionClassification Classification { get; private set; }
	}
}
