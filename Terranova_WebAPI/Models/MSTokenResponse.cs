using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Terranova_WebAPI.Models
{
    public class MSTokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }
}
