using RoyalVillaDTO;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb.Services
{
    public class VillaService :BaseService, IVillaService
    {
   
        private  const string APIEndPoint="api/villa";


        public VillaService(IHttpClientFactory httpClient, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor)
        {
 
        }

        public Task<T?> CreateVillaAsync<T>(VillaCreateDTO villaCreateDTO)
        {
            return SendAsync<T>(new() { Url=$"{APIEndPoint}", Data=villaCreateDTO, ApiType=SD.ApiType.POST});
        }

        public Task<T?> DeleteAsync<T>(int id)
        {
            return SendAsync<T>(new() { Url=$"{APIEndPoint}/{id}",ApiType=SD.ApiType.DELETE });
        }

        public Task<T?> GetAllAsync<T>()
        {
            return SendAsync<T>(new() { Url=$"{APIEndPoint}", ApiType=SD.ApiType.GET });
        }

        public Task<T?> GetAsync<T>(int id)
        {
            return SendAsync<T>(new() { Url=$"{APIEndPoint}/{id}",  ApiType=SD.ApiType.GET });
        }

        public Task<T?> UpdateVillaAsync<T>(VillaUpdateDTO villaUpdateDTO)
        {
            return SendAsync<T>(new() { Url=$"{APIEndPoint}/{villaUpdateDTO.Id}", Data=villaUpdateDTO, ApiType=SD.ApiType.PUT });
        }
    }
}
