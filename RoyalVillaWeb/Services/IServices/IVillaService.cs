using RoyalVillaDTO;

namespace RoyalVillaWeb.Services.IServices
{
    public interface IVillaService
    {
        Task<T?> GetAllAsync<T>();
        Task<T?> GetAsync<T>(int id);
        Task<T?> CreateVillaAsync<T>(VillaCreateDTO villaCreateDTO);
        Task<T?> UpdateVillaAsync<T>(VillaUpdateDTO villaUpdateDTO);
        Task<T?> DeleteAsync<T>(int id);
    }
}
