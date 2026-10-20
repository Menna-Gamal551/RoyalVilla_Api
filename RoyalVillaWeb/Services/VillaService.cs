using RoyalVillaDTO;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb.Services
{
    public class VillaService :BaseService, IVillaService
    {
   
        private  const string APIEndPoint="api/villa";


        public VillaService(IHttpClientFactory httpClient) : base(httpClient)
        {
 
        }

        public Task<T?> CreateVillaAsync<T>(VillaCreateDTO villaCreateDTO, string token)
        {
            return SendAsync<T>(new() { Url=$"{APIEndPoint}", Data=villaCreateDTO, ApiType=SD.ApiType.POST, Token=token });
        }

        public Task<T?> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new() { Url=$"{APIEndPoint}/{id}",ApiType=SD.ApiType.DELETE, Token=token });
        }

        public Task<T?> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new() { Url=$"{APIEndPoint}", ApiType=SD.ApiType.GET, Token=token });
        }

        public Task<T?> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new() { Url=$"{APIEndPoint}/{id}",  ApiType=SD.ApiType.GET, Token=token });
        }

        public Task<T?> UpdateVillaAsync<T>(VillaUpdateDTO villaUpdateDTO, string token)
        {
            return SendAsync<T>(new() { Url=$"{APIEndPoint}/{villaUpdateDTO.Id}", Data=villaUpdateDTO, ApiType=SD.ApiType.PUT, Token=token });
        }
    }
}
