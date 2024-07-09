using System;
using System.Collections.Generic;
using System.Text;

namespace Terranova_GraphClient.AuthenticationProvider
{
    public enum AuthenticationExceptionClassification
    {
		None = 0,
		InvalidRequest = 1,
		// Apllication consent is missing, or has been revoked.
		ConsentRequired = 2,
		InvalidClient = 3,
		MsalServiceException = 4
	}
}
