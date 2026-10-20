using RoyalVillaDTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;
using System.Text.Json;

namespace RoyalVillaWeb.Services
{
    public class BaseService : IBaseService
    {
        private IHttpClientFactory _httpClient;
        public ApiResponse<object> ResponseModel { get; set; }
        private readonly static JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };
        public BaseService(IHttpClientFactory httpClient)
        {
            this.ResponseModel = new();
            _httpClient = httpClient;
        }

        public async Task<T?> SendAsync<T>(ApiRequest apiRequest)
        {
            var client = _httpClient.CreateClient("RoyalVillaApi");
            var message = new HttpRequestMessage
            {
                RequestUri=new Uri(apiRequest.Url, UriKind.Relative),
                Method=GetHttpMethod(apiRequest.ApiType),
            };
            if (apiRequest.Data!=null)
            {
                message.Content=JsonContent.Create(apiRequest.Data, options: jsonSerializerOptions);
            }
            var apiResponse= await client.SendAsync(message);
            return await apiResponse.Content.ReadFromJsonAsync<T>(jsonSerializerOptions);
        }

        private static HttpMethod GetHttpMethod(SD.ApiType apiType)
        {
            return apiType switch
            {
                SD.ApiType.POST => HttpMethod.Post,
                SD.ApiType.PUT => HttpMethod.Put,
                SD.ApiType.DELETE => HttpMethod.Delete,
                _ => HttpMethod.Get
            };
        }
    }
}
