using KargoTakip.Core.Entity;

namespace KargoTakip.Data.Repositories
{
    public interface IKargoDurumGecmisiRepository
    {
        Task<IEnumerable<KargoDurumGecmisi>> GetByKargoIdAsync(int kargoId);
        Task<int> AddAsync(KargoDurumGecmisi kargoDurumGecmisi);
    }
}