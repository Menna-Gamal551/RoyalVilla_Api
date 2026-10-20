using RoyalVillaDTO;

namespace RoyalVillaWeb.Services.IServices
{
    public interface IVillaService
    {
        Task<T?> GetAllAsync<T>(string token);
        Task<T?> GetAsync<T>(int id,string token);
        Task<T?> CreateVillaAsync<T>(VillaCreateDTO villaCreateDTO,string token);
        Task<T?> UpdateVillaAsync<T>(VillaUpdateDTO villaUpdateDTO,string token);
        Task<T?> DeleteAsync<T>(int id,string token);
    }
}
