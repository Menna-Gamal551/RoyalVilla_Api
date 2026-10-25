using RoyalVillaDTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;
using System.Text.Json;
using System.Net.Http.Headers;

namespace RoyalVillaWeb.Services
{
    public class BaseService : IBaseService
    {
        private IHttpClientFactory _httpClient;
        private IHttpContextAccessor _HttpContextAccessor;
        public ApiResponse<object> ResponseModel { get; set; }
        private readonly static JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };
        public BaseService(IHttpClientFactory httpClient, IHttpContextAccessor httpContextAccessor)
        {
            this.ResponseModel = new();
            _httpClient = httpClient;
            _HttpContextAccessor=httpContextAccessor;
        }

        public async Task<T?> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                var client = _httpClient.CreateClient("RoyalVillaApi");
                var message = new HttpRequestMessage
                {
                    RequestUri=new Uri(apiRequest.Url, UriKind.Relative),
                    Method=GetHttpMethod(apiRequest.ApiType),
                };
                var token = _HttpContextAccessor.HttpContext?.Session?.GetString(SD.SessionToken);
              
                if (!string.IsNullOrEmpty(token)) 
                {
                    message.Headers.Authorization=new AuthenticationHeaderValue("Bearer",token);
                }

                if (apiRequest.Data!=null)
                {
                    message.Content=JsonContent.Create(apiRequest.Data, options: jsonSerializerOptions);
                }
                var apiResponse = await client.SendAsync(message);
                return await apiResponse.Content.ReadFromJsonAsync<T>(jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"unexpected error : {ex.Message}");
                return default;
            }
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
