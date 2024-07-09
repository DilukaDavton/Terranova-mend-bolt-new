using System;
using System.Collections.Generic;
using System.Text;

namespace Terranova_GraphClient.Settings
{
    public class OBOSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public List<string> Scopes { get; set; }
    }
}
