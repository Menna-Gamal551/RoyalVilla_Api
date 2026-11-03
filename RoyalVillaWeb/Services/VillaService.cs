using RoyalVillaDTO;
using RoyalVillaWeb.Extentions;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb.Services
{
    public class VillaService :BaseService, IVillaService
    {
   
        private  const string APIEndPoint=$"api/{SD.CurrentAPIVersion}/villa";


        public VillaService(IHttpClientFactory httpClient, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor)
        {
 
        }

        public Task<T?> CreateVillaAsync<T>(VillaCreateDTO villaCreateDTO)
        {
            var formData = villaCreateDTO.ToMultipartFormData();
            return SendAsync<T>(new() { Url=$"{APIEndPoint}", Data=formData, ApiType=SD.ApiType.POST});
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
            var formData = villaUpdateDTO.ToMultipartFormData();
            return SendAsync<T>(new() { Url=$"{APIEndPoint}/{villaUpdateDTO.Id}", Data=formData, ApiType=SD.ApiType.PUT });
        }
    }
}
