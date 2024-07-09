using System;
using System.Collections.Generic;
using System.Text;

namespace Terranova_GraphClient
{
    public interface IOnBehalfOfUserFactory
    {
        IExchangeClient CreateClient(string bootstrapToken);
    }
}
