using RoyalVillaDTO;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private const string APIEndPoint = "api/auth";
        public AuthService(IHttpClientFactory httpClient,IHttpContextAccessor httpContextAccessor):base(httpClient,httpContextAccessor) { }
       
        public  Task<T?> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
        {
            return  SendAsync<T>(new() {Url=$"{APIEndPoint}/Login",Data=loginRequestDTO,ApiType=SD.ApiType.POST });
        }

        public Task<T?> RegisterAsync<T>(RegisterationRequestDTO registerationRequestDTO)
        {
            return SendAsync<T>(new()  { Url=$"{APIEndPoint}/Register", Data=registerationRequestDTO, ApiType=SD.ApiType.POST });
        }
    }
}
