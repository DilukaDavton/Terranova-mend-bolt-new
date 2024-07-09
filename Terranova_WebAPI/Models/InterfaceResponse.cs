namespace Terranova_WebAPI.Models
{
    public class InterfaceResponse
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public TokenViewModel TokenInfo { get; set; }
    }
}
