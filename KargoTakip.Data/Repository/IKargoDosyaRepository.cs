using KargoTakip.Core.Entity;

namespace KargoTakip.Data.Repositories
{
    public interface IKargoDosyaRepository
    {
        Task<IEnumerable<KargoDosya>> GetByKargoIdAsync(int kargoId);
        Task<KargoDosya?> GetByIdAsync(int kargoDosyaId);
        Task<int> AddAsync(KargoDosya kargoDosya);
        Task<bool> DeleteAsync(int kargoDosyaId);
    }
}