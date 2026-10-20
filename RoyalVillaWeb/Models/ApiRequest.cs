namespace RoyalVillaWeb.Models
{
    public class ApiRequest
    {
        public SD.ApiType ApiType { get; set; }
        public string? Url { get; set; }
        public string ?  Token { get; set; }
        public object? Data { get; set; }
    }
}
