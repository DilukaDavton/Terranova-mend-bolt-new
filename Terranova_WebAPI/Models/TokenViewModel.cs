using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Terranova_WebAPI.Models
{
    public class TokenViewModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
